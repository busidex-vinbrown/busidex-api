using System.Web.Mvc;
using System.Web.Security;
using Busidex.BL.Interfaces;
using Busidex.DAL;
using Busidex4.Models;

namespace BusidexJS.Controllers
{
    public class AccountController : Controller
    {
        private enum AccountErrorContentKeys
        {
            Error_IncorrectUserNameOrPassword,
            Error_CreatingAccount,
            Error_InactiveAccount
        }

        private readonly ISettingsRepository _settingsRepository;
        private readonly IAccountRepository _accountRepository;

        public AccountController(IAccountRepository accountRepository, ICardRepository cardRepository, ISettingsRepository settingsRepository)
        {
            _settingsRepository = settingsRepository;
        }



        //[AllowAnonymous]
        //public ActionResult Login(string token)
        //{
        //    return ContextDependentLoginView(token);
        //}

        [AllowAnonymous]
        public JsonResult MobileLogin(string userName, string password)
        {
            long memberId = -1;
            try
            {
                if (Membership.ValidateUser(userName, password))
                {
                    var membershipUser = Membership.GetUser(userName);

                    if (membershipUser != null && membershipUser.ProviderUserKey != null)
                    {
                        UserAccount account =
                            _accountRepository.GetUserAccountByUserId((long)membershipUser.ProviderUserKey);

                        if (account != null && account.Active) // check for an active account
                        {
                            memberId = (long)membershipUser.ProviderUserKey;
                        }
                    }
                }
            }
            catch
            {
                memberId = -2;
            }
            Response.AppendHeader("Access-Control-Allow-Origin", "*");
            return Json(new { MemberId = memberId }, JsonRequestBehavior.AllowGet);
        }

        //[AllowAnonymous]
        //[HttpPost]
        //public JsonResult JsonLogin(LoginModel model, string returnUrl)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        if (Membership.ValidateUser(model.UserName, model.Password))
        //        {
        //            var membershipUser = Membership.GetUser(model.UserName);

        //            if (membershipUser != null && membershipUser.ProviderUserKey != null)
        //            {
        //                UserAccount account =
        //                    _accountRepository.GetUserAccountByUserId((long)membershipUser.ProviderUserKey);

        //                if (account == null || !account.Active) // check for an active account
        //                {
        //                    ModelState.AddModelError("", ContentProvider.GetContent(AccountErrorContentKeys.Error_InactiveAccount.ToString()));
        //                    return Json(new { errors = GetErrorsFromModelState() });
        //                }
        //            }

        //            FormsAuthentication.SetAuthCookie(model.UserName, model.RememberMe);
        //            if (string.IsNullOrEmpty(returnUrl))
        //            {
        //                if (membershipUser != null)
        //                {
        //                    var providerUserKey = membershipUser.ProviderUserKey;
        //                    if (providerUserKey != null)
        //                    {
        //                        if (Request.QueryString["ReturnUrl"] != null)
        //                        {
        //                            Response.Redirect(Request.QueryString["ReturnUrl"]);
        //                        }
        //                        else
        //                        {
        //                            const int PAGE_IDX_HOME = 1;
        //                            Dictionary<int, Page> pages = _accountRepository.GetAllSitePages().ToDictionary(p => p.PageId, p => p);
        //                            BusidexUser user = _accountRepository.GetBusidexUserById((long) providerUserKey);
        //                            if (user.Settings != null)
        //                            {

        //                                if (user.Settings.StartPage != null &&
        //                                    pages.ContainsKey(user.Settings.StartPage.Value))
        //                                    returnUrl = "/" + pages[user.Settings.StartPage.Value].ControllerName + "/" +
        //                                                pages[user.Settings.StartPage.Value].Action;
        //                            }
        //                            else
        //                            {
        //                                _settingsRepository.AddDefaultUserSetting(user);
        //                                returnUrl = "/" + pages[PAGE_IDX_HOME].ControllerName + "/" +
        //                                            pages[PAGE_IDX_HOME].Action;
        //                            }
        //                        }
        //                    }
        //                }
        //            }

        //            return Json(new { success = true, redirect = returnUrl });
        //        }
        //       // ModelState.AddModelError("", ContentProvider.GetContent(AccountErrorContentKeys.Error_IncorrectUserNameOrPassword.ToString()));
        //    }

        //    // If we got this far, something failed
        //    //return Json(new { errors = GetErrorsFromModelState() });
        //}

        [AllowAnonymous]
        [HttpPost]
        public long Login(LoginModel model)
        {
            long userId = -1;
            if (ModelState.IsValid)
            {
                if (Membership.ValidateUser(model.UserName, model.Password))
                {
                   // FormsAuthentication.SetAuthCookie(model.UserName, model.RememberMe);

                    // redirect to the user's preferred page
                    MembershipUser membershipUser = Membership.GetUser(model.UserName);
                    if (membershipUser != null)
                    {
                        object providerUserKey = membershipUser.ProviderUserKey;
                        if (providerUserKey != null)
                        {
                            BusidexUser user =
                                _accountRepository.GetBusidexUserById((long)providerUserKey);

                            userId = user.UserId;
                            //var card = _cardRepository.GetCardByToken(model.CardOwnerToken);

                            //if (card != null && !string.IsNullOrEmpty(model.CardOwnerToken))
                            //{
                            //    return RedirectToAction("ConfirmMyCard", "Card", new { token = model.CardOwnerToken });
                            //}

                            //if (Request.Form["ReturnUrl"] != null && !string.IsNullOrEmpty(Request.Form["ReturnUrl"]))
                            //{
                            //    Response.Redirect(Request.Form["ReturnUrl"]);
                            //}
                            //else
                            //{
                            //    if (user.Settings != null)
                            //    {
                            //        Dictionary<int, Page> pages =
                            //            _accountRepository.GetAllSitePages().ToDictionary(p => p.PageId, p => p);
                            //        if (user.Settings.StartPage != null &&
                            //             pages.ContainsKey(user.Settings.StartPage.Value))
                            //            return RedirectToAction(pages[user.Settings.StartPage.Value].Action,
                            //                                      pages[user.Settings.StartPage.Value].
                            //                                          ControllerName);
                            //    }
                            //}
                        }
                    }

                    

                }
                string key = AccountErrorContentKeys.Error_IncorrectUserNameOrPassword.ToString();
                //ModelState.AddModelError("", ContentProvider.GetContent(key));
            }

            // If we got this far, something failed, redisplay form
            return userId;
        }

        //public ActionResult LogOff()
        //{
        //    FormsAuthentication.SignOut();
        //    Session.Clear();
        //    Session.Abandon();

        //    return RedirectToAction("Index", "Home");
        //}

    //    [AllowAnonymous]
    //    [HttpPost]
    //    public ActionResult JsonRegister(RegisterModel model)
    //    {
    //        if (ModelState.IsValid)
    //        {
    //            // Attempt to register the user
    //            MembershipCreateStatus createStatus;
    //            var user = Membership.CreateUser(model.UserName, model.Password, model.Email, null, null, true, null, out createStatus);

    //            if (user != null && createStatus == MembershipCreateStatus.Success)
    //            {
    //                FormsAuthentication.SetAuthCookie(model.UserName, createPersistentCookie: false);
    //                return Json(new { success = true });
    //            }
    //            ModelState.AddModelError("", ErrorCodeToString(createStatus));
    //        }

    //        // If we got this far, something failed
    //        return Json(new { errors = GetErrorsFromModelState() });
    //    }

    //    [AllowAnonymous]
    //    public ActionResult RecoverUserName()
    //    {
    //        return View();
    //    }

    //    [AllowAnonymous]
    //    public ActionResult SendUserNameReminderEmail()
    //    {
    //        var model = new RecoverUserNameModel();
    //        TryUpdateModel(model);

    //        UserAccount account = _accountRepository.GetUserAccountByEmail(model.Email);

    //        var template = _accountRepository.GetEmailTemplate(EmailTemplateCode.Username);

    //        template.Body = template.Body.Replace("###", account.BusidexUser.UserName);

    //        var communication = new Communication
    //                                {
    //                                    EmailTemplate = template,
    //                                    Email = model.Email,
    //                                    DateSent = DateTime.UtcNow,
    //                                    UserId = account.UserId,
    //                                    Failed = false
    //                                };

    //        try
    //        {
    //            var con = ConfigurationManager.ConnectionStrings["StorageConnection"].ConnectionString;

    //            var storageAccount = CloudStorageAccount.Parse(con);

    //            // Create the queue client.
    //            var queueClient = storageAccount.CreateCloudQueueClient();

    //            // Retrieve a reference to a queue.
    //            var queue = queueClient.GetQueueReference("email");

    //            // Create the queue if it doesn't already exist.
    //            queue.CreateIfNotExist();

    //            // Create a message and add it to the queue.
    //            var settings = new JsonSerializerSettings
    //            {
    //                TypeNameHandling = TypeNameHandling.All,
    //                TypeNameAssemblyFormat = FormatterAssemblyStyle.Simple,
    //                Formatting = Formatting.Indented
    //            };

    //            var jsMessage = JsonConvert.SerializeObject(communication, settings);
    //            var message = new CloudQueueMessage(jsMessage);

    //            queue.AddMessage(message);

    //        }
    //        catch
    //        {
    //            communication.Failed = true;
    //        }
    //        finally
    //        {
    //            _accountRepository.SaveCommunication(communication);
    //        }

    //        return RedirectToAction("Login");
    //    }

    //    [AllowAnonymous]
    //    public ActionResult Register(string token)
    //    {
    //        if (Request.IsAuthenticated)
    //        {
    //            FormsAuthentication.SignOut();
    //            Session.Clear();
    //            Session.Abandon();
    //        }

    //        ViewBag.Plans = _accountRepository.GetActivePlans().OrderBy(p => p.DisplayOrder).ToList();

    //        // If the user is coming here via a link sent with a cardOwnerToken, this will be needed
    //        // as part of the registration. 

    //        var model = new RegisterModel { CardOwnerToken = token, HumanQuestion = ConfigurationManager.AppSettings["HumanQuestion"], IsValid = true };
    //        var card = _cardRepository.GetCardByToken(token);
    //        if (card != null)
    //        {
    //            model.FrontFileId = card.FrontFileId;
    //            model.FrontFileType = card.FrontFileType;
    //            model.StepHash = "step0";
    //        }

    //        return View(model);
    //    }

    //    [AllowAnonymous]
    //    [HttpPost]
    //    public ActionResult Register(RegisterModel model)
    //    {
    //        ViewBag.Plans = _accountRepository.GetActivePlans();
    //        model.StepHash = "step_done";
    //        model.IsValid = false;

    //        if (model.HumanAnswer != ConfigurationManager.AppSettings["HumanAnswer"])
    //        {
    //            ModelState.AddModelError("nothuman", "Sorry, Just not sure if you're human. Try again.");
    //            model.StepHash = "step1";
    //            model.Agree = false;
    //        }
    //        if (ModelState.IsValid)
    //        {
    //            // Attempt to register the user
    //            var ua = new UserAccount();
    //            if (TryUpdateModel(ua))
    //            {
    //                MembershipCreateStatus createStatus;
    //                MembershipUser u = Membership.CreateUser(model.UserName, model.Password, model.Email, null, null, true, null, out createStatus);

    //                if (createStatus == MembershipCreateStatus.Success)
    //                {
    //                    if (u != null && u.ProviderUserKey != null)
    //                    {
    //                        BusidexUser bu = _accountRepository.GetBusidexUserById((long)u.ProviderUserKey);

    //                        ua.UserId = bu.UserId;

    //                        _accountRepository.AddUserAccount(ua);

    //                        if (!string.IsNullOrEmpty(model.CardOwnerToken))
    //                        {
    //                            CardDetailModel card = _cardRepository.GetCardByToken(model.CardOwnerToken);
    //                            if (card != null)
    //                            {
    //                                _cardRepository.SaveCardOwner(card.CardId, bu.UserId);
    //                                _cardRepository.AddToMyBusidex(card.CardId, bu.UserId);
    //                                _cardRepository.AddSendersCardToMyBusidex(model.CardOwnerToken, bu.UserId);
    //                            }

    //                        }

    //                        if (Request.Url != null)
    //                        {
    //                            SendConfirmationEmail(bu.UserId, model.Email);
    //                            model.IsValid = true;
    //                        }
    //                    }
    //                }
    //                else
    //                {
    //                    string errorKey;
    //                    switch (createStatus)
    //                    {
    //                        case MembershipCreateStatus.DuplicateEmail:
    //                            {
    //                                errorKey = "Email";
    //                                break;
    //                            }
    //                        case MembershipCreateStatus.InvalidEmail:
    //                            {
    //                                errorKey = "Email";
    //                                break;
    //                            }
    //                        case MembershipCreateStatus.InvalidPassword:
    //                            {
    //                                errorKey = "Password";
    //                                break;
    //                            }
    //                        case MembershipCreateStatus.InvalidUserName:
    //                            {
    //                                errorKey = "UserName";
    //                                break;
    //                            }
    //                        case MembershipCreateStatus.DuplicateUserName:
    //                            {
    //                                errorKey = "UserName";
    //                                break;
    //                            }
    //                        default:
    //                            {
    //                                errorKey = "UserName";
    //                                break;
    //                            }
    //                    }
    //                    ModelState.AddModelError(errorKey, ErrorCodeToString(createStatus));
    //                    model.StepHash = "step1";
    //                    model.Agree = false;
    //                }
    //            }
    //            else
    //            {
    //                string key = AccountErrorContentKeys.Error_CreatingAccount.ToString();
    //                ModelState.AddModelError("UserName", ContentProvider.GetContent(key));
    //                model.StepHash = "step1";
    //                model.Agree = false;
    //            }
    //        }

    //        FormsAuthentication.SignOut();

    //        return View(model);
    //    }

    //    public void SendInviteEmail(string name, string email)
    //    {

    //        var token = ConfigurationManager.AppSettings["InviteToken"];

    //        var template = _accountRepository.GetEmailTemplate(EmailTemplateCode.Invitation);

    //        var user = Membership.GetUser();
    //        template.Subject = template.Subject.Replace("###", name);
    //        template.Body = template.Body.Replace("###", token);

    //        if (user != null && user.ProviderUserKey != null)
    //        {
    //            var communication = new Communication
    //                                    {
    //                                        EmailTemplate = template,
    //                                        Email = email,
    //                                        DateSent = DateTime.UtcNow,
    //                                        UserId = (long)user.ProviderUserKey,
    //                                        Failed = false,
    //                                        EmailTemplateId = template.EmailTemplateId
    //                                    };

    //            try
    //            {
    //                SendEmail(communication);
    //            }
    //            catch (Exception ex)
    //            {
    //                string exception = ex.Message;
    //                communication.Failed = true;
    //            }
    //            finally
    //            {
    //                _accountRepository.SaveCommunication(communication);
    //            }
    //        }
    //    }

    //    private void SendConfirmationEmail(long userId, string email)
    //    {
    //        var token = Guid.NewGuid();
    //        _accountRepository.SaveUserAccountToken(userId, token);

    //        var template = _accountRepository.GetEmailTemplate(EmailTemplateCode.Registration);

    //        template.Body = template.Body.Replace("###", token.ToString());

    //        var communication = new Communication
    //                                {
    //                                    EmailTemplate = template,
    //                                    Email = email,
    //                                    DateSent = DateTime.UtcNow,
    //                                    UserId = userId,
    //                                    Failed = false,
    //                                    EmailTemplateId = template.EmailTemplateId
    //                                };

    //        try
    //        {
    //            SendEmail(communication);
    //        }
    //        catch (Exception ex)
    //        {
    //            string exception = ex.Message;
    //            communication.Failed = true;
    //        }
    //        finally
    //        {
    //            _accountRepository.SaveCommunication(communication);
    //        }
    //    }

    //    [AllowAnonymous]
    //    public ActionResult RegistrationComplete(string token)
    //    {
    //        if (string.IsNullOrEmpty(token))
    //        {
    //            return RedirectToAction("Index", "Home");
    //        }

    //        var tokenGuid = new Guid(token);
    //        UserAccount userAccount = _accountRepository.GetUserAccountByToken(tokenGuid);

    //        var model = new UserAccountModel
    //        {
    //            UserAccount = userAccount,
    //            Email = string.Empty
    //        };

    //        if (userAccount != null)
    //        {
    //            MembershipUser user = Membership.GetUser(userAccount.UserId);

    //            if (user != null)
    //            {
    //                model.Email = user.Email;

    //                if (!userAccount.Active)
    //                {
    //                    _accountRepository.ActivateUserAccount(new Guid(token));
    //                }
    //                ViewBag.ValidToken = true;
    //            }
    //        }
    //        return View(model);
    //    }

    //    public ActionResult ChangePassword()
    //    {
    //        return View();
    //    }

    //    [HttpPost]
    //    public ActionResult ChangePassword(ChangePasswordModel model)
    //    {
    //        if (ModelState.IsValid)
    //        {
    //            // ChangePassword will throw an exception rather
    //            // than return false in certain failure scenarios.
    //            bool changePasswordSucceeded = false;
    //            try
    //            {
    //                MembershipUser currentUser = Membership.GetUser(User.Identity.Name, userIsOnline: true);
    //                if (currentUser != null)
    //                    changePasswordSucceeded = currentUser.ChangePassword(model.OldPassword, model.NewPassword);
    //            }
    //            catch (Exception)
    //            {
    //                changePasswordSucceeded = false;
    //            }

    //            if (changePasswordSucceeded)
    //            {
    //                return RedirectToAction("ChangePasswordSuccess");
    //            }

    //            string key = AccountErrorContentKeys.Error_IncorrectUserNameOrPassword.ToString();

    //            ModelState.AddModelError("", ContentProvider.GetContent(key));
    //        }

    //        // If we got this far, something failed, redisplay form
    //        return View(model);
    //    }

    //    public ActionResult ChangePasswordSuccess()
    //    {
    //        return View();
    //    }

    //    private ActionResult ContextDependentLoginView(string token)
    //    {
    //        string actionName = "Login";// ControllerContext.RouteData.GetRequiredString("action");
    //        if (Request.QueryString["content"] != null)
    //        {
    //            ViewBag.FormAction = "JsonLogin";
    //            return PartialView(token);
    //        }
    //        ViewBag.FormAction = actionName;
    //        ViewBag.CardToken = token;
    //        if (!string.IsNullOrEmpty(token))
    //        {
    //            ViewBag.UnownedCard = _cardRepository.GetCardByToken(token);
    //        }
    //        return View();
    //    }

    //    private IEnumerable<string> GetErrorsFromModelState()
    //    {
    //        return ModelState.SelectMany(x => x.Value.Errors.Select(error => error.ErrorMessage));
    //    }

    //    #region Status Codes

    //    private static string ErrorCodeToString(MembershipCreateStatus createStatus)
    //    {
    //        // See http://go.microsoft.com/fwlink/?LinkID=177550 for
    //        // a full list of status codes.
    //        switch (createStatus)
    //        {
    //            case MembershipCreateStatus.DuplicateUserName:
    //                return "User name already exists. Please enter a different user name.";

    //            case MembershipCreateStatus.DuplicateEmail:
    //                return
    //                    "A user name for that e-mail address already exists. Please enter a different e-mail address.";

    //            case MembershipCreateStatus.InvalidPassword:
    //                return "The password provided is invalid. Please enter a valid password value.";

    //            case MembershipCreateStatus.InvalidEmail:
    //                return "The e-mail address provided is invalid. Please check the value and try again.";

    //            case MembershipCreateStatus.InvalidAnswer:
    //                return "The password retrieval answer provided is invalid. Please check the value and try again.";

    //            case MembershipCreateStatus.InvalidQuestion:
    //                return "The password retrieval question provided is invalid. Please check the value and try again.";

    //            case MembershipCreateStatus.InvalidUserName:
    //                return "The user name provided is invalid. Please check the value and try again.";

    //            case MembershipCreateStatus.ProviderError:
    //                return
    //                    "The authentication provider returned an error. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

    //            case MembershipCreateStatus.UserRejected:
    //                return
    //                    "The user creation request has been canceled. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

    //            default:
    //                return
    //                    "An unknown error occurred. Please verify your entry and try again. If the problem persists, please contact your system administrator.";
    //        }
    //    }

    //    #endregion
    }
}