using System;
using System.ComponentModel;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;
using System.Web.Mvc;
using System.Web.Security;
using Busidex.Api.DataAccess.DTO;
using Busidex.Api.DataServices.Interfaces;
using Busidex.Api.Models;

namespace Busidex.Api.Controllers
{
    [RequireHttps]
    [EnableCors("*", "*", "*")]
    public class UserController : BaseApiController
    {
        private readonly IAccountRepository _accountRepository;

        public UserController(IAccountRepository accountRepository, ICardRepository cardRepository)
        {
            _accountRepository = accountRepository;
            _cardRepository = cardRepository;
        }

        public HttpResponseMessage Get(string name)
        {
            var user = Membership.GetUser(name);
            if (user == null)
            {
                string userName = Membership.GetUserNameByEmail(name);
                user = Membership.GetUser(userName);
            }

            return new HttpResponseMessage
            {
                Content = new JsonContent(new
                {
                    User = user   
                })
            };
        }

        [System.Web.Http.HttpPost]
        public HttpResponseMessage RecoverUserName(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                return new HttpResponseMessage
                {
                    Content = new JsonContent(new
                    {
                        Success = false,
                        Message = "Invalid Email",
                    }),
                    StatusCode = HttpStatusCode.BadRequest
                };
            }

            var user = _accountRepository.GetUserByEmail(email);
            if (user == null)
            {
                return new HttpResponseMessage
                {
                    Content = new JsonContent(new
                    {
                        Success = false,
                        Message = "User not found",
                    }),
                    StatusCode = HttpStatusCode.NotFound
                };
            }

            var template = _accountRepository.GetEmailTemplate(EmailTemplateCode.Username);

            template.Body = template.Body.Replace("###", HttpUtility.UrlEncode(user.UserName));

            var communication = new Communication
            {
                EmailTemplate = template,
                Email = email,
                DateSent = DateTime.UtcNow,
                UserId = 0,
                Failed = false,
                EmailTemplateId = template.EmailTemplateId
            };

            SendEmail(communication);

            return new HttpResponseMessage
            {
                Content = new JsonContent(new
                {
                    Success = true,
                    Message = "Your username has been sent to you.",
                }),
                StatusCode = HttpStatusCode.OK
            };

        }

        [System.Web.Http.HttpPut]
        public HttpResponseMessage RecoverPassword(string email)
        {
            string sanitizedEmail = email.Trim();
            const int MAX_EMAIL_LENGTH = 256;

            if (sanitizedEmail.Length > MAX_EMAIL_LENGTH)
            {
                sanitizedEmail = sanitizedEmail.Substring(0, MAX_EMAIL_LENGTH);
            }

            if (string.IsNullOrEmpty(sanitizedEmail))
            {
                return new HttpResponseMessage
                {
                    Content = new JsonContent(new
                    {
                        Success = false,
                        Message = "Invalid Email",
                    }),
                    StatusCode = HttpStatusCode.BadRequest
                };
            }

            var user = _accountRepository.GetUserByEmail(sanitizedEmail);
            if (user == null)
            {
                return new HttpResponseMessage
                {
                    Content = new JsonContent(new
                    {
                        Success = false,
                        Message = "User not found",
                    }),
                    StatusCode = HttpStatusCode.NotFound
                };
            }

            string newPassword = Membership.GeneratePassword(Membership.MinRequiredPasswordLength, Membership.MinRequiredNonAlphanumericCharacters);
            var encodedPassword = EncodeString(newPassword);
            bool success = _accountRepository.UpdatePassword(user.UserName, encodedPassword);

            if (!success)
            {
                return new HttpResponseMessage
                {
                    Content = new JsonContent(new
                    {
                        Success = false,
                        Message = "There was a problem resetting the user password",
                    }),
                    StatusCode = HttpStatusCode.InternalServerError
                };
            }


            string data = sanitizedEmail + " " + newPassword;
            string encryptedData = Convert.ToBase64String(GetBytes(data));
            var template = _accountRepository.GetEmailTemplate(EmailTemplateCode.PasswordRecover);

            template.Body = template.Body.Replace("###", HttpUtility.UrlEncode(encryptedData));

            var communication = new Communication
            {
                EmailTemplate = template,
                Email = sanitizedEmail,
                DateSent = DateTime.UtcNow,
                UserId = 0,
                Failed = false,
                EmailTemplateId = template.EmailTemplateId
            };

            SendEmail(communication);

            return new HttpResponseMessage
            {
                Content = new JsonContent(new
                {
                    Success = true,
                    Message = "Your password has been reset",
                }),
                StatusCode = HttpStatusCode.OK
            };
        }

        [System.Web.Http.HttpPut]
        public HttpResponseMessage ChangeUserName(long userId, string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return new HttpResponseMessage
                {
                    Content = new JsonContent(new
                    {
                        Success = false,
                        Message = "Invalid parameters",
                    }),
                    StatusCode = HttpStatusCode.BadRequest
                };
            }

            if ( userId <= 0)
            {
                userId = ValidateUser();
                if (userId <= 0)
                {
                    return new HttpResponseMessage
                    {
                        Content = new JsonContent(new
                        {
                            Success = false,
                            Message = "Invalid parameters",
                        }),
                        StatusCode = HttpStatusCode.BadRequest
                    };
                }
            }

            var existing = Membership.GetUser(name);
            if (existing != null)
            {
                return new HttpResponseMessage
                {
                    Content = new JsonContent(new
                    {
                        Success = false,
                        Message = "Username already exists",
                    }),
                    StatusCode = HttpStatusCode.BadRequest
                };
            }

            var user = Membership.GetUser(userId);
            if (user == null)
            {
                return new HttpResponseMessage
                {
                    Content = new JsonContent(new
                    {
                        Success = false,
                        Message = "User not found",
                    }),
                    StatusCode = HttpStatusCode.NotFound
                };
            }

            try
            {
                var userNameUpdated = _accountRepository.UpdateUserName(userId, name);

                if (userNameUpdated)
                {
                    return new HttpResponseMessage
                    {
                        Content = new JsonContent(new
                        {
                            Success = true,
                            Message = "Username updated",
                        }),
                        StatusCode = HttpStatusCode.OK
                    };
                }
            }
            catch (Exception ex)
            {
                _cardRepository.SaveApplicationError(ex, userId);
            }

            return new HttpResponseMessage
            {
                Content = new JsonContent(new
                {
                    Success = true,
                    Message = "Couldn't update your user name",
                }),
                StatusCode = HttpStatusCode.InternalServerError
            };
        }

        [System.Web.Http.HttpPut]
        public HttpResponseMessage ChangeEmail(string email)
        {
            long userId = ValidateUser();
            if (userId <= 0)
            {
                return new HttpResponseMessage
                {
                    Content = null,
                    StatusCode = HttpStatusCode.Unauthorized
                };
            }

            if (string.IsNullOrEmpty(email) || !email.Contains("@"))
            {
                return new HttpResponseMessage
                {
                    Content = new JsonContent(new
                    {
                        Success = false,
                        Message = "Invalid Email",
                    }),
                    StatusCode = HttpStatusCode.BadRequest
                };
            }

            try
            {
                var existingEmail = _accountRepository.GetUserByEmail(email);
                if (existingEmail != null)
                {
                    return new HttpResponseMessage
                    {
                        Content = new JsonContent(new
                        {
                            Success = false,
                            Message = "Email Updated",
                        }),
                        StatusCode = HttpStatusCode.BadRequest,
                        ReasonPhrase = "Email already exists"
                    };
                }

                var updated = _accountRepository.UpdateEmail(userId, email);
                if (updated)
                {
                    return new HttpResponseMessage
                    {
                        Content = new JsonContent(new
                        {
                            Success = true,
                            Message = "Email Updated",
                        }),
                        StatusCode = HttpStatusCode.OK
                    };
                }
            }
            catch (Exception ex)
            {
                _cardRepository.SaveApplicationError(ex, userId);
            }


            return new HttpResponseMessage
            {
                Content = new JsonContent(new
                {
                    Success = false,
                    Message = "Email Not Updated",
                }),
                StatusCode = HttpStatusCode.BadRequest
            };
        }

        [System.Web.Http.HttpPut]
        public HttpResponseMessage UpdateDeviceType(DeviceType deviceType)
        {
            long userId = 0; 

            
            try
            {
                userId = ValidateUser();
                if (userId <= 0)
                {
                    return new HttpResponseMessage
                    {
                        Content = null,
                        StatusCode = HttpStatusCode.Unauthorized
                    };
                }


                _accountRepository.AddUserDeviceType(userId, deviceType);

                //_cardRepository.SaveApplicationError("Saving device type: " + deviceType.ToString(), string.Empty, string.Empty, userId);

                return new HttpResponseMessage
                {
                    Content = new JsonContent(new
                    {
                        Success = true
                    }),
                    StatusCode = HttpStatusCode.OK
                };
            }
            catch (InvalidEnumArgumentException ex)
            {
                _cardRepository.SaveApplicationError(ex, userId);
                return new HttpResponseMessage
                {
                    Content = new JsonContent(new
                    {
                        Success = false
                    }),
                    StatusCode = HttpStatusCode.BadRequest
                };
            }
            catch (Exception ex)
            {
                _cardRepository.SaveApplicationError(ex, userId);
                return new HttpResponseMessage
                {
                    Content = new JsonContent(new
                    {
                        Success = false
                    }),
                    StatusCode = HttpStatusCode.InternalServerError
                };
            }

        }

        [System.Web.Http.HttpPut]
        public HttpResponseMessage UpdateUser([FromBody] UserDTO userDto)
        {
            var userId = ValidateUser();
            if (userId <= 0 || userId != userDto?.UserId)
            {
                return new HttpResponseMessage
                {
                    Content = null,
                    StatusCode = HttpStatusCode.Unauthorized
                };
            }

            var user = _accountRepository.GetBusidexUserById(userId);
            if (user.UserAccount.DisplayName != userDto.DisplayName)
            {
                _accountRepository.UpdateDisplayName(user.UserAccount.UserAccountId, userDto.DisplayName);
            }
            
            if (user.Email != userDto.Email)
            {
                _accountRepository.UpdateEmail(userId, userDto.Email);
            }

            return new HttpResponseMessage
            {
                Content = new JsonContent(new
                {
                    Success = true
                }),
                StatusCode = HttpStatusCode.OK
            };;
        }
    }
}
