
namespace Busidex.Providers
{
    using System.Web.Security;
    using System.Configuration.Provider;
    using System.Collections.Specialized;
    using System;
    using System.Data;
    using System.Configuration;
    using System.Diagnostics;
    using System.Security.Cryptography;
    using System.Data.SqlClient;
    using System.Text;
    using System.Web.Configuration;


    public sealed class BusidexMembershipProvider : MembershipProvider
    {

        //private readonly BusidexCacheProvider _busidexCacheProvider;
        //
        // Global connection string, generated password length, generic exception message, event log info.
        //
        private const string BUSIDEX_PROVIDER_NAME = "BusidexMembershipProvider";

        private const int NEW_PASSWORD_LENGTH = 16;
        private const string EVENT_SOURCE = BUSIDEX_PROVIDER_NAME;
        private const string EVENT_LOG = "Application";
        private const string EXCEPTION_MESSAGE = "An exception occurred. Please check the Event Log.";
        private string _connectionString;

        //
        // Used when determining encryption key values.
        //

        public BusidexMembershipProvider()
        {
           // _busidexCacheProvider = new BusidexCacheProvider();
        }

        private MachineKeySection _machineKey;

        //
        // If false, exceptions are thrown to the caller. If true,
        // exceptions are written to the event log.
        //

        private bool _pWriteExceptionsToEventLog;

        public bool WriteExceptionsToEventLog
        {
            get { return _pWriteExceptionsToEventLog; }
            set { _pWriteExceptionsToEventLog = value; }
        }


        //
        // System.Configuration.Provider.ProviderBase.Initialize Method
        //

        public override void Initialize(string name, NameValueCollection config)
        {
            //
            // Initialize values from web.config.
            //

            if (config == null)
                throw new ArgumentNullException("config");

            if (name.Length == 0)
                name = BUSIDEX_PROVIDER_NAME;

            if (String.IsNullOrEmpty(config["description"]))
            {
                config.Remove("description");
                config.Add("description", "Busidex Membership provider");
            }

            // Initialize the abstract base class.
            base.Initialize(name, config);

            _pApplicationName = GetConfigValue(config["applicationName"],
                                               System.Web.Hosting.HostingEnvironment.ApplicationVirtualPath);
            _pMaxInvalidPasswordAttempts = Convert.ToInt32(GetConfigValue(config["maxInvalidPasswordAttempts"], "5"));
            _pPasswordAttemptWindow = Convert.ToInt32(GetConfigValue(config["passwordAttemptWindow"], "10"));
            _pMinRequiredNonAlphanumericCharacters =
                Convert.ToInt32(GetConfigValue(config["minRequiredNonAlphanumericCharacters"], "1"));
            _pMinRequiredPasswordLength = Convert.ToInt32(GetConfigValue(config["minRequiredPasswordLength"], "7"));
            _pPasswordStrengthRegularExpression =
                Convert.ToString(GetConfigValue(config["passwordStrengthRegularExpression"], ""));
            _pEnablePasswordReset = Convert.ToBoolean(GetConfigValue(config["enablePasswordReset"], "true"));
            _pEnablePasswordRetrieval = Convert.ToBoolean(GetConfigValue(config["enablePasswordRetrieval"], "true"));
            _pRequiresQuestionAndAnswer = Convert.ToBoolean(GetConfigValue(config["requiresQuestionAndAnswer"], "false"));
            _pRequiresUniqueEmail = Convert.ToBoolean(GetConfigValue(config["requiresUniqueEmail"], "true"));
            _pWriteExceptionsToEventLog = false;
            // Convert.ToBoolean(GetConfigValue(config["writeExceptionsToEventLog"], "false"));

            string tempFormat = config["passwordFormat"] ?? "Hashed";

            switch (tempFormat)
            {
                case "Hashed":
                    _pPasswordFormat = MembershipPasswordFormat.Hashed;
                    break;
                case "Encrypted":
                    _pPasswordFormat = MembershipPasswordFormat.Encrypted;
                    break;
                case "Clear":
                    _pPasswordFormat = MembershipPasswordFormat.Clear;
                    break;
                default:
                    throw new ProviderException("Password format not supported.");
            }

            //
            // Initialize SqlConnection.
            //

            ConnectionStringSettings connectionStringSettings =
                ConfigurationManager.ConnectionStrings[config["connectionStringName"]];

            if (connectionStringSettings == null || connectionStringSettings.ConnectionString.Trim() == "")
            {
                throw new ProviderException("Connection string cannot be blank.");
            }

            _connectionString = connectionStringSettings.ConnectionString;


            // Get encryption and decryption key information from the configuration.
            Configuration cfg =
                WebConfigurationManager.OpenWebConfiguration(
                    System.Web.Hosting.HostingEnvironment.ApplicationVirtualPath);
            _machineKey = (MachineKeySection)cfg.GetSection("system.web/machineKey");

            if (_machineKey.ValidationKey.Contains("AutoGenerate"))
                if (PasswordFormat != MembershipPasswordFormat.Clear)
                    throw new ProviderException("Hashed or Encrypted passwords " +
                                                "are not supported with auto-generated keys.");
        }


        //
        // A helper function to retrieve config values from the configuration file.
        //

        private string GetConfigValue(string configValue, string defaultValue)
        {
            if (String.IsNullOrEmpty(configValue))
                return defaultValue;

            return configValue;
        }


        //
        // System.Web.Security.MembershipProvider properties.
        //


        private string _pApplicationName;
        private bool _pEnablePasswordReset;
        private bool _pEnablePasswordRetrieval;
        private bool _pRequiresQuestionAndAnswer;
        private bool _pRequiresUniqueEmail;
        private int _pMaxInvalidPasswordAttempts;
        private int _pPasswordAttemptWindow;
        private MembershipPasswordFormat _pPasswordFormat;

        public override string ApplicationName
        {
            get { return _pApplicationName; }
            set { _pApplicationName = value; }
        }

        public override bool EnablePasswordReset
        {
            get { return _pEnablePasswordReset; }
        }


        public override bool EnablePasswordRetrieval
        {
            get { return _pEnablePasswordRetrieval; }
        }


        public override bool RequiresQuestionAndAnswer
        {
            get { return _pRequiresQuestionAndAnswer; }
        }


        public override bool RequiresUniqueEmail
        {
            get { return _pRequiresUniqueEmail; }
        }


        public override int MaxInvalidPasswordAttempts
        {
            get { return _pMaxInvalidPasswordAttempts; }
        }


        public override int PasswordAttemptWindow
        {
            get { return _pPasswordAttemptWindow; }
        }


        public override MembershipPasswordFormat PasswordFormat
        {
            get { return _pPasswordFormat; }
        }

        private int _pMinRequiredNonAlphanumericCharacters;

        public override int MinRequiredNonAlphanumericCharacters
        {
            get { return _pMinRequiredNonAlphanumericCharacters; }
        }

        private int _pMinRequiredPasswordLength;

        public override int MinRequiredPasswordLength
        {
            get { return _pMinRequiredPasswordLength; }
        }

        private string _pPasswordStrengthRegularExpression;

        public override string PasswordStrengthRegularExpression
        {
            get { return _pPasswordStrengthRegularExpression; }
        }

        //
        // System.Web.Security.MembershipProvider methods.
        //

        //
        // MembershipProvider.ChangePassword
        //

        public override bool ChangePassword(string username, string oldPwd, string newPwd)
        {

            if (!ValidateUser(username, oldPwd))
                return false;

            //var args =
            //    new ValidatePasswordEventArgs(username, newPwd, true);

            //OnValidatingPassword(args);

            //if (args.Cancel)
            //{
            //    if (args.FailureInformation != null)
            //    {
            //        throw args.FailureInformation;
            //    }
            //    throw new MembershipPasswordException("Change password canceled due to new password validation failure.");
            //}

            //MembershipUser mu = GetUser(username, true);
            //if (mu != null)
            //{
            //    return mu.ChangePassword(oldPwd, newPwd);
            //}
            //return false;
            var conn = new SqlConnection(_connectionString);
            var cmd = new SqlCommand("usp_busidex_Membership_ChangePassword", conn) { CommandType = CommandType.StoredProcedure };

            cmd.Parameters.Add("@Password", SqlDbType.VarChar, 255).Value = EncodePassword(newPwd);
            cmd.Parameters.Add("@LastPasswordChangedDate", SqlDbType.DateTime).Value = DateTime.Now;
            cmd.Parameters.Add("@Username", SqlDbType.VarChar, 255).Value = username;
            cmd.Parameters.Add("@ApplicationName", SqlDbType.VarChar, 255).Value = _pApplicationName;


            int rowsAffected;

            try
            {
                conn.Open();

                rowsAffected = cmd.ExecuteNonQuery();
            }
            catch (SqlException e)
            {
                if (WriteExceptionsToEventLog)
                {
                    WriteToEventLog(e, "ChangePassword");

                    throw new ProviderException("Noooooooooooooo!!!!");
                }
                else
                {
                    throw;
                }
            }
            finally
            {
                conn.Close();
            }

            if (rowsAffected > 0)
            {
                return true;
            }

            return false;
        }



        //
        // MembershipProvider.ChangePasswordQuestionAndAnswer
        //

        public override bool ChangePasswordQuestionAndAnswer(string username,
                                                             string password,
                                                             string newPwdQuestion,
                                                             string newPwdAnswer)
        {
            if (!ValidateUser(username, password))
                return false;

            var conn = new SqlConnection(_connectionString);
            var cmd = new SqlCommand("usp_busidex_Membership_ChangePasswordQuestionAndAnswer", conn);

            cmd.Parameters.Add("@Question", SqlDbType.VarChar, 255).Value = newPwdQuestion;
            cmd.Parameters.Add("@Answer", SqlDbType.VarChar, 255).Value = EncodePassword(newPwdAnswer);
            cmd.Parameters.Add("@Username", SqlDbType.VarChar, 255).Value = username;
            cmd.Parameters.Add("@ApplicationName", SqlDbType.VarChar, 255).Value = _pApplicationName;
            cmd.CommandType = CommandType.StoredProcedure;

            int rowsAffected = 0;

            try
            {
                conn.Open();

                rowsAffected = cmd.ExecuteNonQuery();
            }
            catch (SqlException e)
            {
                if (WriteExceptionsToEventLog)
                {
                    WriteToEventLog(e, "ChangePasswordQuestionAndAnswer");

                    throw new ProviderException(EXCEPTION_MESSAGE);
                }
                else
                {
                    throw;
                }
            }
            catch (Exception e)
            {
                WriteToEventLog(e, "ChangePasswordQuestionAndAnswer");
            }
            finally
            {
                conn.Close();
            }

            if (rowsAffected > 0)
            {
                return true;
            }

            return false;
        }



        //
        // MembershipProvider.CreateUser
        //

        private string GenerateSalt()
        {
            var buf = new byte[16];
            (new RNGCryptoServiceProvider()).GetBytes(buf);
            return Convert.ToBase64String(buf);
        }

        public override MembershipUser CreateUser(string username,
                                                  string password,
                                                  string email,
                                                  string passwordQuestion,
                                                  string passwordAnswer,
                                                  bool isApproved,
                                                  object providerUserKey,
                                                  out MembershipCreateStatus status)
        {

            var args = new ValidatePasswordEventArgs(username, password, true);

            OnValidatingPassword(args);


            if (args.Cancel)
            {
                status = MembershipCreateStatus.InvalidPassword;
                return null;
            }

            if (RequiresUniqueEmail && GetUserNameByEmail(email) != "")
            {
                status = MembershipCreateStatus.DuplicateEmail;
                return null;
            }

            MembershipUser u = GetUser(username, false);

            if (u == null)
            {
                DateTime createDate = DateTime.Now;

                //if (providerUserKey == null) {
                //    providerUserKey = Guid.NewGuid();
                //} else {
                //    if (!(providerUserKey is Guid)) {
                //        status = MembershipCreateStatus.InvalidProviderUserKey;
                //        return null;
                //    }
                //}
                var conn = new SqlConnection(_connectionString);
                var cmd = new SqlCommand("aspnet_Membership_CreateUser", conn) { CommandType = CommandType.StoredProcedure };

                cmd.Parameters.Add("@ApplicationName", SqlDbType.NVarChar, 256).Value = ApplicationName;
                cmd.Parameters.Add("@Username", SqlDbType.NVarChar, 256).Value = username;
                cmd.Parameters.Add("@Password", SqlDbType.NVarChar, 128).Value = EncodePassword(password);
                cmd.Parameters.Add("@PasswordSalt", SqlDbType.NVarChar, 128).Value = GenerateSalt();
                cmd.Parameters.Add("@Email", SqlDbType.NVarChar, 256).Value = email;
                cmd.Parameters.Add("@PasswordQuestion", SqlDbType.NVarChar, 256).Value = "Who?"; // passwordQuestion;
                cmd.Parameters.Add("@PasswordAnswer", SqlDbType.NVarChar, 128).Value = EncodePassword("Me");
                cmd.Parameters.Add("@IsApproved", SqlDbType.Bit).Value = isApproved;
                cmd.Parameters.Add("@CurrentTimeUtc", SqlDbType.DateTime).Value = createDate;
                cmd.Parameters.Add("@CreateDate", SqlDbType.DateTime).Value = createDate;
                cmd.Parameters.Add("@UniqueEmail", SqlDbType.Int).Value = 0;
                cmd.Parameters.Add("@PasswordFormat", SqlDbType.Int).Value = 0;

                var oParmUserId = new SqlParameter("@UserId", SqlDbType.BigInt) { Direction = ParameterDirection.Output, Value = 0 };
                cmd.Parameters.Add(oParmUserId);

                try
                {
                    conn.Open();

                    int recAdded = cmd.ExecuteNonQuery();

                    status = recAdded > 0 ? MembershipCreateStatus.Success : MembershipCreateStatus.UserRejected;
                }
                catch (SqlException e)
                {
                    if (WriteExceptionsToEventLog)
                    {
                        WriteToEventLog(e, "CreateUser");
                    }

                    status = MembershipCreateStatus.ProviderError;

                }
                catch (Exception e)
                {
                    WriteToEventLog(e, "CreateUser");
                    status = MembershipCreateStatus.ProviderError;
                }
                finally
                {
                    conn.Close();
                }


                return GetUser(oParmUserId.Value, false);

            }
            status = MembershipCreateStatus.DuplicateUserName;


            return null;
        }



        //
        // MembershipProvider.DeleteUser
        //
        /// <summary>
        /// ALTER PROCEDURE [dbo].[aspnet_Users_DeleteUser]
        //@ApplicationName  nvarchar(256),
        //@UserName         nvarchar(256),
        //@TablesToDeleteFrom int,
        //@NumTablesDeletedFrom int OUTPUT
        /// </summary>
        /// <param name="username"></param>
        /// <param name="deleteAllRelatedData"></param>
        /// <returns></returns>
        public override bool DeleteUser(string username, bool deleteAllRelatedData)
        {

            var conn = new SqlConnection(_connectionString);
            var cmd = new SqlCommand("aspnet_Users_DeleteUser", conn) { CommandType = CommandType.StoredProcedure };

            cmd.Parameters.Add("@Username", SqlDbType.NVarChar, 256).Value = username;
            cmd.Parameters.Add("@TablesToDeleteFrom", SqlDbType.NVarChar, 256).Value = username;
            cmd.Parameters.Add("@ApplicationName", SqlDbType.VarChar, 256).Value = _pApplicationName;

            var oParm = new SqlParameter("@NumTablesDeletedFrom", SqlDbType.Int) { Direction = ParameterDirection.Output };
            cmd.Parameters.Add(oParm);

            int rowsAffected = 0;

            try
            {
                conn.Open();

                rowsAffected = cmd.ExecuteNonQuery();

                if (deleteAllRelatedData)
                {
                    // Process commands to delete all data for the user in the database.
                }
            }
            catch (SqlException e)
            {
                if (WriteExceptionsToEventLog)
                {
                    WriteToEventLog(e, "DeleteUser");

                    throw new ProviderException(EXCEPTION_MESSAGE);
                }
                else
                {
                    throw;
                }
            }
            catch (Exception e)
            {
                WriteToEventLog(e, "DeleteUser");
            }
            finally
            {
                conn.Close();
            }

            if (rowsAffected > 0)
                return true;

            return false;
        }



        //
        // MembershipProvider.GetAllUsers
        //

        public override MembershipUserCollection GetAllUsers(int pageIndex, int pageSize, out int totalRecords)
        {
            var conn = new SqlConnection(_connectionString);
            var cmd = new SqlCommand("aspnet_Membership_GetAllUsers", conn) { CommandType = CommandType.StoredProcedure };

            cmd.Parameters.Add("@PageIndex", SqlDbType.Int, pageIndex);
            cmd.Parameters.Add("@PageSize", SqlDbType.Int, pageSize);
            cmd.Parameters.Add("@ApplicationName", SqlDbType.VarChar, 256).Value = _pApplicationName;

            var users = new MembershipUserCollection();

            SqlDataReader reader = null;
            totalRecords = 0;

            try
            {
                conn.Open();
                //totalRecords = (int)cmd.ExecuteScalar();

                //if (totalRecords <= 0) { return users; }

                //cmd.CommandText = "aspnet_Membership_GetAllUsers";
                //cmd.CommandType = CommandType.StoredProcedure;

                reader = cmd.ExecuteReader();

                int counter = 0;
                int startIndex = pageSize * pageIndex;
                int endIndex = startIndex + pageSize - 1;

                while (reader.Read())
                {
                    if (counter >= startIndex)
                    {
                        MembershipUser u = GetUserFromReader(reader);
                        users.Add(u);
                    }

                    if (counter >= endIndex)
                    {
                        cmd.Cancel();
                    }

                    counter++;
                }
                totalRecords = users.Count;

            }
            catch (SqlException e)
            {
                if (WriteExceptionsToEventLog)
                {
                    WriteToEventLog(e, "GetAllUsers ");

                    throw new ProviderException(EXCEPTION_MESSAGE);
                }
                else
                {
                    throw;
                }
            }
            catch (Exception e)
            {
                WriteToEventLog(e, "GetAllUsers");
            }
            finally
            {
                if (reader != null)
                {
                    reader.Close();
                }
                conn.Close();
            }

            return users;
        }


        //
        // MembershipProvider.GetNumberOfUsersOnline
        //

        public override int GetNumberOfUsersOnline()
        {

            return Membership.GetNumberOfUsersOnline();
            /*
            TimeSpan onlineSpan = new TimeSpan(0, System.Web.Security.Membership.UserIsOnlineTimeWindow, 0);
            DateTime compareTime = DateTime.Now.Subtract(onlineSpan);

            SqlConnection conn = new SqlConnection(connectionString);
            SqlCommand cmd = new SqlCommand("SELECT Count(*) FROM Users " +
                    " WHERE LastActivityDate > ? AND ApplicationName = ?", conn);

            cmd.Parameters.Add("@CompareDate", SqlDbType.DateTime).Value = compareTime;
            cmd.Parameters.Add("@ApplicationName", SqlDbType.VarChar, 255).Value = pApplicationName;

            int numOnline = 0;

            try {
                conn.Open();

                numOnline = (int)cmd.ExecuteScalar();
            } catch (SqlException e) {
                if (WriteExceptionsToEventLog) {
                    WriteToEventLog(e, "GetNumberOfUsersOnline");

                    throw new ProviderException(exceptionMessage);
                } else {
                    throw e;
                }
            } finally {
                conn.Close();
            }

            return numOnline;*/
        }



        //
        // MembershipProvider.GetPassword
        //

        public override string GetPassword(string username, string answer)
        {
            if (!EnablePasswordRetrieval)
            {
                throw new ProviderException("Password Retrieval Not Enabled.");
            }

            if (PasswordFormat == MembershipPasswordFormat.Hashed)
            {
                throw new ProviderException("Cannot retrieve Hashed passwords.");
            }

            var conn = new SqlConnection(_connectionString);
            var cmd = new SqlCommand("usp_busidex_Membership_GetPassword", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add("@Username", SqlDbType.VarChar, 255).Value = username;
            cmd.Parameters.Add("@ApplicationName", SqlDbType.VarChar, 255).Value = _pApplicationName;

            string password = "";
            string passwordAnswer = "";
            SqlDataReader reader = null;

            try
            {
                conn.Open();

                reader = cmd.ExecuteReader(CommandBehavior.SingleRow);

                if (reader.HasRows)
                {
                    reader.Read();

                    if (reader.GetBoolean(2))
                        throw new MembershipPasswordException("The supplied user is locked out.");

                    password = reader.GetString(0);
                    passwordAnswer = reader.GetString(1);
                }
                else
                {
                    throw new MembershipPasswordException("The supplied user name is not found.");
                }
            }
            catch (SqlException e)
            {
                if (WriteExceptionsToEventLog)
                {
                    WriteToEventLog(e, "GetPassword");

                    throw new ProviderException(EXCEPTION_MESSAGE);
                }
                else
                {
                    throw;
                }
            }
            catch (Exception e)
            {
                WriteToEventLog(e, "GetPassword");
            }
            finally
            {
                if (reader != null)
                {
                    reader.Close();
                }
                conn.Close();
            }


            if (RequiresQuestionAndAnswer && !CheckPassword(answer, passwordAnswer))
            {
                UpdateFailureCount(username, "passwordAnswer");

                throw new MembershipPasswordException("Incorrect password answer.");
            }


            if (PasswordFormat == MembershipPasswordFormat.Encrypted)
            {
                password = UnEncodePassword(password);
            }

            return password;
        }

        public override MembershipUser GetUser(string username, bool userIsOnline)
        {
            //var user = _busidexCacheProvider.GetFromCache(BusidexCacheProvider.CachKeys.CurrentUser) as MembershipUser;
            //if (user != null)
            //{
            //    return user;
            //}

            var conn = new SqlConnection(_connectionString);
            var conn2 = new SqlConnection(_connectionString);
            var cmd = new SqlCommand("busidex_Membership_GetUserByName", conn) { CommandType = CommandType.StoredProcedure };

            cmd.Parameters.Add("@Username", SqlDbType.VarChar, 250).Value = username;
            cmd.Parameters.Add("@ApplicationName", SqlDbType.VarChar, 250).Value = _pApplicationName;
            cmd.Parameters.Add("@CurrentTimeUtc", SqlDbType.DateTime).Value = DateTime.UtcNow;

            MembershipUser u = null;
            SqlDataReader reader = null;

            try
            {
                conn.Open();
                conn2.Open();
                reader = cmd.ExecuteReader();

                if (reader.HasRows)
                {
                    reader.Read();
                    u = GetUserFromReader(reader);

                    if (userIsOnline)
                    {
                        var updateCmd = new SqlCommand("busidex_Membership_UpdateUser", conn2) { CommandType = CommandType.StoredProcedure };

                        updateCmd.Parameters.Add("@ApplicationName", SqlDbType.NVarChar, 256).Value = _pApplicationName;
                        updateCmd.Parameters.Add("@Username", SqlDbType.VarChar, 256).Value = username;
                        updateCmd.Parameters.Add("@Email", SqlDbType.NVarChar, 256).Value = u.Email;
                        updateCmd.Parameters.Add("@Comment", SqlDbType.NText).Value = "";
                        updateCmd.Parameters.Add("@IsApproved", SqlDbType.Bit).Value = 1;
                        updateCmd.Parameters.Add("@LastLoginDate", SqlDbType.DateTime).Value = DateTime.Now;
                        updateCmd.Parameters.Add("@LastActivityDate", SqlDbType.DateTime).Value = DateTime.Now;
                        updateCmd.Parameters.Add("@UniqueEmail", SqlDbType.Int).Value = 0;
                        updateCmd.Parameters.Add("@CurrentTimeUtc", SqlDbType.DateTime).Value = DateTime.UtcNow;

                        updateCmd.ExecuteNonQuery();
                    }
                }

            }
            catch (SqlException e)
            {
                if (WriteExceptionsToEventLog)
                {
                    WriteToEventLog(e, "GetUser(String, Boolean)");

                    throw new ProviderException(EXCEPTION_MESSAGE);
                }
                else
                {
                    throw;
                }
            }
            catch (Exception e)
            {
                WriteToEventLog(e, "GetUser(String, Boolean)");
            }
            finally
            {
                if (reader != null)
                {
                    reader.Close();
                }

                conn.Close();
                conn2.Close();
            }

            //_busidexCacheProvider.UpdateCache(BusidexCacheProvider.CachKeys.CurrentUser, u);
            return u;
        }


        //
        // MembershipProvider.GetUser(object, bool)
        //

        public override MembershipUser GetUser(object providerUserKey, bool userIsOnline)
        {
            //var user = _busidexCacheProvider.GetFromCache(BusidexCacheProvider.CachKeys.CurrentUser) as MembershipUser;
            //if (user != null)
            //{
            //    return user;
            //}

            var conn = new SqlConnection(_connectionString);
            var conn2 = new SqlConnection(_connectionString);
            var cmd = new SqlCommand("busidex_Membership_GetUserByUserId", conn);

            cmd.Parameters.Add("@UserId", SqlDbType.BigInt).Value = providerUserKey;
            cmd.Parameters.Add("@CurrentTimeUtc", SqlDbType.BigInt).Value = providerUserKey;
            cmd.CommandType = CommandType.StoredProcedure;

            MembershipUser u = null;
            SqlDataReader reader = null;

            try
            {
                conn.Open();
                conn2.Open();
                reader = cmd.ExecuteReader();

                if (reader.HasRows)
                {
                    reader.Read();
                    u = GetUserFromReader(reader);

                    if (userIsOnline)
                    {
                        var updateCmd = new SqlCommand("busidex_Membership_UpdateUser", conn2) { CommandType = CommandType.StoredProcedure };
                        updateCmd.Parameters.Add("@ApplicationName", SqlDbType.NVarChar, 256).Value = _pApplicationName;
                        updateCmd.Parameters.Add("@UserName", SqlDbType.NVarChar, 256).Value = u.UserName;
                        updateCmd.Parameters.Add("@Email", SqlDbType.NVarChar, 256).Value = u.Email;
                        updateCmd.Parameters.Add("@Comment", SqlDbType.NText).Value = "";
                        updateCmd.Parameters.Add("@IsApproved", SqlDbType.Bit).Value = 1;
                        updateCmd.Parameters.Add("@LastLoginDate", SqlDbType.DateTime).Value = DateTime.Now;
                        updateCmd.Parameters.Add("@LastActivityDate", SqlDbType.DateTime).Value = DateTime.Now;
                        updateCmd.Parameters.Add("@UniqueEmail", SqlDbType.Int).Value = 0;
                        updateCmd.Parameters.Add("@CurrentTimeUtc", SqlDbType.DateTime).Value = DateTime.UtcNow;

                        updateCmd.ExecuteNonQuery();
                    }
                }

            }
            catch (SqlException e)
            {
                if (WriteExceptionsToEventLog)
                {
                    WriteToEventLog(e, "GetUser(Object, Boolean)");

                    throw new ProviderException(EXCEPTION_MESSAGE);
                }
                else
                {
                    throw;
                }
            }
            catch (Exception e)
            {
                WriteToEventLog(e, "GetUser(Object, Boolean)");
            }
            finally
            {
                if (reader != null)
                {
                    reader.Close();
                }

                conn.Close();
                conn2.Close();
            }

            //_busidexCacheProvider.UpdateCache(BusidexCacheProvider.CachKeys.CurrentUser, u);

            return u;
        }


        //
        // GetUserFromReader
        //    A helper function that takes the current row from the SqlDataReader
        // and hydrates a MembershiUser from the values. Called by the 
        // MembershipUser.GetUser implementation.
        //

        /*
         * u.UserId, m.Email, m.PasswordQuestion, m.Comment, m.IsApproved,
            m.CreateDate, m.LastLoginDate, u.LastActivityDate,
            m.LastPasswordChangedDate, u.UserName, m.IsLockedOut,
            m.LastLockoutDate
         * 
         * u.UserId, m.Email, m.PasswordQuestion, m.Comment, m.IsApproved,
                m.CreateDate, m.LastLoginDate, u.LastActivityDate, m.LastPasswordChangedDate,
                u.UserId, m.IsLockedOut,m.LastLockoutDate
         */

        private MembershipUser GetUserFromReader(SqlDataReader reader)
        {
            object providerUserKey = reader.GetValue(0);
            string username = reader.GetString(9);
            string email = reader.GetString(1);

            const string passwordQuestion = "";
            //if (reader.GetValue(3) != DBNull.Value)
            //    passwordQuestion = reader.GetString(3);

            //string comment = "";
            //if (reader.GetValue(4) != DBNull.Value)
            //    comment = reader.GetString(4);

            bool isLockedOut = reader.GetBoolean(10);
            DateTime creationDate = reader.GetDateTime(5);

            var lastLoginDate = new DateTime();
            if (reader.GetValue(6) != DBNull.Value)
                lastLoginDate = reader.GetDateTime(6);

            DateTime lastActivityDate = reader.GetDateTime(7);
            DateTime lastPasswordChangedDate = reader.GetDateTime(8);

            var lastLockedOutDate = new DateTime();
            if (reader.GetValue(11) != DBNull.Value)
                lastLockedOutDate = reader.GetDateTime(11);

            var u = new MembershipUser(Name,
                                       username,
                                       providerUserKey,
                                       email,
                                       passwordQuestion,
                                       "",
                                       true,
                                       isLockedOut,
                                       creationDate,
                                       lastLoginDate,
                                       lastActivityDate,
                                       lastPasswordChangedDate,
                                       lastLockedOutDate);

            return u;
        }


        //
        // MembershipProvider.UnlockUser
        //

        public override bool UnlockUser(string username)
        {
            var conn = new SqlConnection(_connectionString);
            var cmd = new SqlCommand("usp_busidex_Membership_UnlockUser", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add("@LastLockedOutDate", SqlDbType.DateTime).Value = DateTime.Now;
            cmd.Parameters.Add("@Username", SqlDbType.VarChar, 255).Value = username;
            cmd.Parameters.Add("@ApplicationName", SqlDbType.VarChar, 255).Value = _pApplicationName;

            int rowsAffected = 0;

            try
            {
                conn.Open();

                rowsAffected = cmd.ExecuteNonQuery();
            }
            catch (SqlException e)
            {
                if (WriteExceptionsToEventLog)
                {
                    WriteToEventLog(e, "UnlockUser");

                    throw new ProviderException(EXCEPTION_MESSAGE);
                }
                else
                {
                    throw;
                }
            }
            catch (Exception e)
            {
                WriteToEventLog(e, "UnlockUser");
            }
            finally
            {
                conn.Close();
            }

            if (rowsAffected > 0)
                return true;

            return false;
        }


        //
        // MembershipProvider.GetUserNameByEmail
        //

        public override string GetUserNameByEmail(string email)
        {
            var conn = new SqlConnection(_connectionString);
            var cmd = new SqlCommand("usp_busidex_Membership_GetUsernameByEmail", conn);

            cmd.Parameters.Add("@Email", SqlDbType.VarChar, 128).Value = email;
            cmd.Parameters.Add("@ApplicationName", SqlDbType.VarChar, 255).Value = _pApplicationName;
            cmd.CommandType = CommandType.StoredProcedure;

            string username = "";

            try
            {
                conn.Open();

                username = (string)cmd.ExecuteScalar();
            }
            catch (SqlException e)
            {
                if (WriteExceptionsToEventLog)
                {
                    WriteToEventLog(e, "GetUserNameByEmail");

                    throw new ProviderException(EXCEPTION_MESSAGE);
                }
                else
                {
                    throw;
                }
            }
            catch (Exception e)
            {
                WriteToEventLog(e, "GetUserNameByEmail");
            }
            finally
            {
                conn.Close();
            }

            return username ?? ("");
        }




        //
        // MembershipProvider.ResetPassword
        //

        public override string ResetPassword(string username, string answer)
        {
            if (!EnablePasswordReset)
            {
                throw new NotSupportedException("Password reset is not enabled.");
            }

            if (answer == null && RequiresQuestionAndAnswer)
            {
                UpdateFailureCount(username, "passwordAnswer");

                throw new ProviderException("Password answer required for password reset.");
            }

            string newPassword =
                Membership.GeneratePassword(NEW_PASSWORD_LENGTH, MinRequiredNonAlphanumericCharacters);


            var args =
                new ValidatePasswordEventArgs(username, newPassword, true);

            OnValidatingPassword(args);

            if (args.Cancel)
                if (args.FailureInformation != null)
                    throw args.FailureInformation;
                else
                    throw new MembershipPasswordException("Reset password canceled due to password validation failure.");


            var conn = new SqlConnection(_connectionString);
            var cmd = new SqlCommand("usp_busidex_Membership_GetPasswordAnswer", conn);

            cmd.Parameters.Add("@Username", SqlDbType.VarChar, 255).Value = username;
            cmd.Parameters.Add("@ApplicationName", SqlDbType.VarChar, 255).Value = _pApplicationName;

            int rowsAffected = 0;
            SqlDataReader reader = null;

            try
            {
                conn.Open();

                reader = cmd.ExecuteReader(CommandBehavior.SingleRow);

                string passwordAnswer;
                if (reader.HasRows)
                {
                    reader.Read();

                    if (reader.GetBoolean(1))
                        throw new MembershipPasswordException("The supplied user is locked out.");

                    passwordAnswer = reader.GetString(0);
                }
                else
                {
                    throw new MembershipPasswordException("The supplied user name is not found.");
                }

                if (RequiresQuestionAndAnswer && !CheckPassword(answer, passwordAnswer))
                {
                    UpdateFailureCount(username, "passwordAnswer");

                    throw new MembershipPasswordException("Incorrect password answer.");
                }

                var updateCmd = new SqlCommand("usp_busidex_Membership_UpdatePassword",
                                               conn);

                updateCmd.CommandType = CommandType.StoredProcedure;
                updateCmd.Parameters.Add("@Password", SqlDbType.VarChar, 255).Value = EncodePassword(newPassword);
                updateCmd.Parameters.Add("@LastPasswordChangedDate", SqlDbType.DateTime).Value = DateTime.Now;
                updateCmd.Parameters.Add("@Username", SqlDbType.VarChar, 255).Value = username;
                updateCmd.Parameters.Add("@ApplicationName", SqlDbType.VarChar, 255).Value = _pApplicationName;

                rowsAffected = updateCmd.ExecuteNonQuery();
            }
            catch (SqlException e)
            {
                if (WriteExceptionsToEventLog)
                {
                    WriteToEventLog(e, "ResetPassword");

                    throw new ProviderException(EXCEPTION_MESSAGE);
                }
                else
                {
                    throw;
                }
            }
            catch (Exception e)
            {
                WriteToEventLog(e, "ResetPassword");
            }
            finally
            {
                if (reader != null)
                {
                    reader.Close();
                }
                conn.Close();
            }

            if (rowsAffected > 0)
            {
                return newPassword;
            }
            throw new MembershipPasswordException("User not found, or user is locked out. Password not Reset.");
        }


        //
        // MembershipProvider.UpdateUser
        //

        public override void UpdateUser(MembershipUser user)
        {
            var conn = new SqlConnection(_connectionString);
            var cmd = new SqlCommand("busidex_Membership_UpdateUser", conn);

            cmd.Parameters.Add("@Email", SqlDbType.VarChar, 128).Value = user.Email;
            cmd.Parameters.Add("@Comment", SqlDbType.VarChar, 255).Value = user.Comment;
            cmd.Parameters.Add("@IsApproved", SqlDbType.Bit).Value = user.IsApproved;
            cmd.Parameters.Add("@Username", SqlDbType.VarChar, 255).Value = user.UserName;
            cmd.Parameters.Add("@ApplicationName", SqlDbType.VarChar, 255).Value = _pApplicationName;
            cmd.Parameters.Add("@LastLoginDate", SqlDbType.DateTime).Value = user.LastLoginDate;
            cmd.Parameters.Add("@LastActivityDate", SqlDbType.DateTime).Value = user.LastActivityDate;
            cmd.CommandType = CommandType.StoredProcedure;

            try
            {
                conn.Open();

                cmd.ExecuteNonQuery();
            }
            catch (SqlException e)
            {
                if (WriteExceptionsToEventLog)
                {
                    WriteToEventLog(e, "UpdateUser");

                    throw new ProviderException(EXCEPTION_MESSAGE);
                }
                else
                {
                    throw;
                }
            }
            catch (Exception e)
            {
                WriteToEventLog(e, "UpdateUser");
            }
            finally
            {
                conn.Close();
            }
        }


        //
        // MembershipProvider.ValidateUser
        //
        /*
         *     @ApplicationName                nvarchar(256),
               @UserName                       nvarchar(256),
               @UpdateLastLoginActivityDate    bit,
               @CurrentTimeUtc                 datetime
         */

        public override bool ValidateUser(string username, string password)
        {
            bool isValid = false;

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                return false;
            }

            var conn = new SqlConnection(_connectionString);
            var conn2 = new SqlConnection(_connectionString);
            var cmd = new SqlCommand("busidex_Membership_GetPasswordWithFormat", conn) { CommandType = CommandType.StoredProcedure };

            cmd.Parameters.Add("@ApplicationName", SqlDbType.NVarChar, 256).Value = _pApplicationName;
            cmd.Parameters.Add("@Username", SqlDbType.NVarChar, 256).Value = username;
            cmd.Parameters.Add("@UpdateLastLoginActivityDate", SqlDbType.Bit).Value = 0;
            cmd.Parameters.Add("@CurrentTimeUtc", SqlDbType.DateTime).Value = DateTime.UtcNow;

            SqlDataReader reader = null;

            try
            {
                conn.Open();

                reader = cmd.ExecuteReader(CommandBehavior.SingleRow);

                string pwd;
                bool isApproved;
                if (reader.HasRows)
                {
                    reader.Read();
                    pwd = reader.GetString(0);
                    isApproved = reader.GetBoolean(5);
                }
                else
                {
                    return false;
                }

                reader.Close();

                if (CheckPassword(password, pwd))
                {
                    if (isApproved)
                    {
                        isValid = true;
                        conn2.Open();
                        MembershipUser u = GetUser(username, false);
                        if (u != null)
                        {
                            var updateCmd = new SqlCommand("busidex_Membership_UpdateUser", conn2) { CommandType = CommandType.StoredProcedure };

                            updateCmd.Parameters.Add("@ApplicationName", SqlDbType.NVarChar, 256).Value =
                                _pApplicationName;
                            updateCmd.Parameters.Add("@Username", SqlDbType.VarChar, 256).Value = username;
                            updateCmd.Parameters.Add("@Email", SqlDbType.NVarChar, 256).Value = u.Email;
                            updateCmd.Parameters.Add("@Comment", SqlDbType.NText).Value = "";
                            updateCmd.Parameters.Add("@IsApproved", SqlDbType.Bit).Value = 1;
                            updateCmd.Parameters.Add("@LastLoginDate", SqlDbType.DateTime).Value = DateTime.Now;
                            updateCmd.Parameters.Add("@LastActivityDate", SqlDbType.DateTime).Value = DateTime.Now;
                            updateCmd.Parameters.Add("@UniqueEmail", SqlDbType.Int).Value = 0;
                            updateCmd.Parameters.Add("@CurrentTimeUtc", SqlDbType.DateTime).Value = DateTime.UtcNow;
                        }
                    }
                }
                else
                {
                    conn.Close();
                    if (conn2.State == ConnectionState.Open)
                    {
                        conn2.Close();
                    }
                    UpdateFailureCount(username, "password");
                }
            }
            catch (SqlException e)
            {
                if (WriteExceptionsToEventLog)
                {
                    WriteToEventLog(e, "ValidateUser");

                    throw new ProviderException(EXCEPTION_MESSAGE);
                }
                else
                {
                    throw;
                }
            }
            catch (Exception e)
            {
                if (WriteExceptionsToEventLog)
                {
                    WriteToEventLog(e, "ValidateUser");
                }
            }
            finally
            {
                if (reader != null)
                {
                    reader.Close();
                }
                conn.Close();
                if (conn2.State == ConnectionState.Open)
                {
                    conn2.Close();
                }
            }

            return isValid;
        }

        public MembershipUser Login(string username, string password)
        {

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                return null;
            }

            var conn = new SqlConnection(_connectionString);
            var conn2 = new SqlConnection(_connectionString);
            var cmd = new SqlCommand("busidex_Membership_GetPasswordWithFormat", conn) { CommandType = CommandType.StoredProcedure };

            cmd.Parameters.Add("@ApplicationName", SqlDbType.NVarChar, 256).Value = _pApplicationName;
            cmd.Parameters.Add("@Username", SqlDbType.NVarChar, 256).Value = username;
            cmd.Parameters.Add("@UpdateLastLoginActivityDate", SqlDbType.Bit).Value = 0;
            cmd.Parameters.Add("@CurrentTimeUtc", SqlDbType.DateTime).Value = DateTime.UtcNow;

            SqlDataReader reader = null;

            MembershipUser user = null;

            try
            {
                conn.Open();

                reader = cmd.ExecuteReader(CommandBehavior.SingleRow);

                string pwd;
                bool isApproved;
                if (reader.HasRows)
                {
                    reader.Read();
                    pwd = reader.GetString(0);
                    isApproved = reader.GetBoolean(5);
                }
                else
                {
                    return null;
                }

                reader.Close();

                if (CheckPassword(password, pwd))
                {
                    if (isApproved)
                    {
                        conn2.Open();
                        MembershipUser u = GetUser(username, false);
                        if (u != null)
                        {
                            var updateCmd = new SqlCommand("busidex_Membership_UpdateUser", conn2) { CommandType = CommandType.StoredProcedure };

                            updateCmd.Parameters.Add("@ApplicationName", SqlDbType.NVarChar, 256).Value =
                                _pApplicationName;
                            updateCmd.Parameters.Add("@Username", SqlDbType.VarChar, 256).Value = username;
                            updateCmd.Parameters.Add("@Email", SqlDbType.NVarChar, 256).Value = u.Email;
                            updateCmd.Parameters.Add("@Comment", SqlDbType.NText).Value = "";
                            updateCmd.Parameters.Add("@IsApproved", SqlDbType.Bit).Value = 1;
                            updateCmd.Parameters.Add("@LastLoginDate", SqlDbType.DateTime).Value = DateTime.Now;
                            updateCmd.Parameters.Add("@LastActivityDate", SqlDbType.DateTime).Value = DateTime.Now;
                            updateCmd.Parameters.Add("@UniqueEmail", SqlDbType.Int).Value = 0;
                            updateCmd.Parameters.Add("@CurrentTimeUtc", SqlDbType.DateTime).Value = DateTime.UtcNow;
                        }
                    }
                }
                else
                {
                    conn.Close();
                    if (conn2.State == ConnectionState.Open)
                    {
                        conn2.Close();
                    }
                    UpdateFailureCount(username, "password");
                }
            }
            catch (SqlException e)
            {
                if (WriteExceptionsToEventLog)
                {
                    WriteToEventLog(e, "ValidateUser");

                    throw new ProviderException(EXCEPTION_MESSAGE);
                }
                else
                {
                    throw;
                }
            }
            catch (Exception e)
            {
                if (WriteExceptionsToEventLog)
                {
                    WriteToEventLog(e, "ValidateUser");
                }
            }
            finally
            {
                if (reader != null)
                {
                    reader.Close();
                }
                conn.Close();
                if (conn2.State == ConnectionState.Open)
                {
                    conn2.Close();
                }
            }

            return user;
        }
        //
        // UpdateFailureCount
        //   A helper method that performs the checks and updates associated with
        // password failure tracking.
        //

        private void UpdateFailureCount(string username, string failureType)
        {
            var conn = new SqlConnection(_connectionString);
            var cmd = new SqlCommand("busidex_Membership_GetFailureCount", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add("@userName", SqlDbType.VarChar, 255).Value = username;
            cmd.Parameters.Add("@appName", SqlDbType.VarChar, 255).Value = _pApplicationName;

            SqlDataReader reader = null;
            var windowStart = new DateTime();
            int failureCount = 0;

            try
            {
                conn.Open();

                reader = cmd.ExecuteReader(CommandBehavior.SingleRow);

                if (reader.HasRows)
                {
                    reader.Read();

                    if (failureType == "password")
                    {
                        failureCount = reader.GetInt32(0);
                        windowStart = reader.GetDateTime(1);
                    }

                    if (failureType == "passwordAnswer")
                    {
                        failureCount = reader.GetInt32(2);
                        windowStart = reader.GetDateTime(3);
                    }
                }

                reader.Close();

                DateTime windowEnd = windowStart.AddMinutes(PasswordAttemptWindow);

                if (failureCount == 0 || DateTime.Now > windowEnd)
                {
                    // First password failure or outside of PasswordAttemptWindow. 
                    // Start a new password failure count from 1 and a new window starting now.

                    if (failureType == "password")
                        cmd.CommandText = "busidex_Membership_UpdateFailureCount";

                    if (failureType == "passwordAnswer")
                        cmd.CommandText = "busidex_Membership_UpdateFailedPasswordAnswerCount";

                    cmd.Parameters.Clear();

                    cmd.Parameters.Add("@failCount", SqlDbType.Int).Value = 1;
                    //cmd.Parameters.Add("@WindowStart", SqlDbType.DateTime).Value = DateTime.Now;
                    cmd.Parameters.Add("@Username", SqlDbType.VarChar, 255).Value = username;
                    cmd.Parameters.Add("@ApplicationName", SqlDbType.VarChar, 255).Value = _pApplicationName;

                    cmd.ExecuteNonQuery();
                }
                else
                {
                    if (failureCount++ >= MaxInvalidPasswordAttempts)
                    {
                        // Password attempts have exceeded the failure threshold. Lock out
                        // the user.

                        cmd.CommandText = "busidex_Membership_UpdateIsLockedOut";

                        cmd.Parameters.Clear();

                        cmd.Parameters.Add("@IsLockedOut", SqlDbType.Bit).Value = true;
                        cmd.Parameters.Add("@LastLockedOutDate", SqlDbType.DateTime).Value = DateTime.Now;
                        cmd.Parameters.Add("@Username", SqlDbType.VarChar, 255).Value = username;
                        cmd.Parameters.Add("@ApplicationName", SqlDbType.VarChar, 255).Value = _pApplicationName;

                        if (cmd.ExecuteNonQuery() < 0)
                            throw new ProviderException("Unable to lock out user.");
                    }
                    else
                    {
                        // Password attempts have not exceeded the failure threshold. Update
                        // the failure counts. Leave the window the same.

                        if (failureType == "password")
                            cmd.CommandText = "busidex_Membership_UpdateFailedPasswordAttemptCount";

                        if (failureType == "passwordAnswer")
                            cmd.CommandText = "busidex_Membership_UpdateFailedPasswordAnswerAttemptCount";

                        cmd.Parameters.Clear();

                        cmd.Parameters.Add("@Count", SqlDbType.Int).Value = failureCount;
                        cmd.Parameters.Add("@Username", SqlDbType.VarChar, 255).Value = username;
                        cmd.Parameters.Add("@ApplicationName", SqlDbType.VarChar, 255).Value = _pApplicationName;

                        if (cmd.ExecuteNonQuery() < 0)
                            throw new ProviderException("Unable to update failure count.");
                    }
                }
            }
            catch (SqlException e)
            {
                if (WriteExceptionsToEventLog)
                {
                    WriteToEventLog(e, "UpdateFailureCount");

                    throw new ProviderException(EXCEPTION_MESSAGE);
                }
                else
                {
                    throw;
                }
            }
            catch (Exception e)
            {
                if (WriteExceptionsToEventLog)
                {
                    WriteToEventLog(e, "UpdateFailureCount");
                }
            }
            finally
            {
                if (reader != null)
                {
                    reader.Close();
                }
                conn.Close();
            }
        }


        //
        // CheckPassword
        //   Compares password values based on the MembershipPasswordFormat.
        //

        private bool CheckPassword(string password, string dbpassword)
        {
            string pass1 = password;
            string pass2 = dbpassword;

            switch (PasswordFormat)
            {
                case MembershipPasswordFormat.Encrypted:
                    pass2 = UnEncodePassword(dbpassword);
                    break;
                case MembershipPasswordFormat.Hashed:
                    pass1 = EncodePassword(password);
                    break;
            }

            if (pass1 == pass2)
            {
                return true;
            }

            return false;
        }


        //
        // EncodePassword
        //   Encrypts, Hashes, or leaves the password clear based on the PasswordFormat.
        //

        private string EncodePassword(string password)
        {

            string encodedPassword = password;

            switch (PasswordFormat)
            {
                case MembershipPasswordFormat.Clear:
                    break;
                case MembershipPasswordFormat.Encrypted:
                    encodedPassword =
                        Convert.ToBase64String(EncryptPassword(Encoding.Unicode.GetBytes(password)));
                    break;
                case MembershipPasswordFormat.Hashed:
                    var hash = new HMACSHA1 { Key = HexToByte(_machineKey.ValidationKey) };
                    encodedPassword =
                        Convert.ToBase64String(hash.ComputeHash(Encoding.Unicode.GetBytes(password)));
                    break;
                default:
                    throw new ProviderException("Unsupported password format.");
            }

            return encodedPassword;
        }


        //
        // UnEncodePassword
        //   Decrypts or leaves the password clear based on the PasswordFormat.
        //

        private string UnEncodePassword(string encodedPassword)
        {
            string password = encodedPassword;

            switch (PasswordFormat)
            {
                case MembershipPasswordFormat.Clear:
                    break;
                case MembershipPasswordFormat.Encrypted:
                    password =
                        Encoding.Unicode.GetString(DecryptPassword(Convert.FromBase64String(password)));
                    break;
                case MembershipPasswordFormat.Hashed:
                    throw new ProviderException("Cannot unencode a hashed password.");
                default:
                    throw new ProviderException("Unsupported password format.");
            }

            return password;
        }

        //
        // HexToByte
        //   Converts a hexadecimal string to a byte array. Used to convert encryption
        // key values from the configuration.
        //

        private byte[] HexToByte(string hexString)
        {
            var returnBytes = new byte[hexString.Length / 2];
            for (int i = 0; i < returnBytes.Length; i++)
                returnBytes[i] = Convert.ToByte(hexString.Substring(i * 2, 2), 16);
            return returnBytes;
        }


        //
        // MembershipProvider.FindUsersByName
        //

        public override MembershipUserCollection FindUsersByName(string usernameToMatch, int pageIndex, int pageSize,
                                                                 out int totalRecords)
        {

            /*
             *     @ApplicationName       nvarchar(256),
                    @UserNameToMatch       nvarchar(256),
                    @PageIndex             int,
                    @PageSize              int
             */
            var conn = new SqlConnection(_connectionString);
            var cmd = new SqlCommand("busidex_Membership_FindUsersByName", conn);
            cmd.Parameters.Add("@ApplicationName", SqlDbType.NVarChar, 256).Value = _pApplicationName;
            cmd.Parameters.Add("@UserNameToMatch", SqlDbType.NVarChar, 256).Value = usernameToMatch;
            cmd.Parameters.Add("@PageIndex", SqlDbType.Int, pageIndex).Value = pageIndex;
            cmd.Parameters.Add("@PageSize", SqlDbType.Int, pageIndex).Value = pageSize;

            var users = new MembershipUserCollection();

            SqlDataReader reader = null;

            try
            {
                cmd.CommandType = CommandType.StoredProcedure;

                conn.Open();
                totalRecords = (int)cmd.ExecuteScalar();

                if (totalRecords <= 0)
                {
                    return users;
                }

                reader = cmd.ExecuteReader();

                int counter = 0;
                int startIndex = pageSize * pageIndex;
                int endIndex = startIndex + pageSize - 1;

                while (reader.Read())
                {
                    if (counter >= startIndex)
                    {
                        MembershipUser u = GetUserFromReader(reader);
                        users.Add(u);
                    }

                    if (counter >= endIndex)
                    {
                        cmd.Cancel();
                    }

                    counter++;
                }
            }
            catch (SqlException e)
            {
                if (WriteExceptionsToEventLog)
                {
                    WriteToEventLog(e, "FindUsersByName: ApplicationName=" + _pApplicationName);

                    throw new ProviderException(EXCEPTION_MESSAGE);
                }
                else
                {
                    throw;
                }
            }
            catch (Exception e)
            {
                if (WriteExceptionsToEventLog)
                {
                    WriteToEventLog(e, "FindUsersByName: ApplicationName=" + _pApplicationName);
                }
                totalRecords = 0;
            }
            finally
            {
                if (reader != null)
                {
                    reader.Close();
                }

                conn.Close();
            }

            return users;
        }

        //
        // MembershipProvider.FindUsersByEmail
        //

        public override MembershipUserCollection FindUsersByEmail(string emailToMatch, int pageIndex, int pageSize,
                                                                  out int totalRecords)
        {
            var conn = new SqlConnection(_connectionString);
            var cmd = new SqlCommand { Connection = conn };
            cmd.Parameters.Add("@EmailSearch", SqlDbType.VarChar, 255).Value = emailToMatch;
            cmd.Parameters.Add("@ApplicationName", SqlDbType.VarChar, 255).Value = _pApplicationName;
            cmd.CommandType = CommandType.StoredProcedure;

            var users = new MembershipUserCollection();

            SqlDataReader reader = null;
            totalRecords = 0;

            try
            {
                conn.Open();
                totalRecords = (int)cmd.ExecuteScalar();

                if (totalRecords <= 0)
                {
                    return users;
                }

                cmd.CommandText = "busidex_Membership_FindUsersByEmail";

                reader = cmd.ExecuteReader();

                int counter = 0;
                int startIndex = pageSize * pageIndex;
                int endIndex = startIndex + pageSize - 1;

                while (reader.Read())
                {
                    if (counter >= startIndex)
                    {
                        MembershipUser u = GetUserFromReader(reader);
                        users.Add(u);
                    }

                    if (counter >= endIndex)
                    {
                        cmd.Cancel();
                    }

                    counter++;
                }
            }
            catch (SqlException e)
            {
                if (WriteExceptionsToEventLog)
                {
                    WriteToEventLog(e, "FindUsersByEmail");

                    throw new ProviderException(EXCEPTION_MESSAGE);
                }
                else
                {
                    throw;
                }
            }
            catch (Exception e)
            {
                if (WriteExceptionsToEventLog)
                {
                    WriteToEventLog(e, "FindUsersByEmail");
                }
            }
            finally
            {
                if (reader != null)
                {
                    reader.Close();
                }

                conn.Close();
            }

            return users;
        }


        //
        // WriteToEventLog
        //   A helper function that writes exception detail to the event log. Exceptions
        // are written to the event log as a security measure to avoid private database
        // details from being returned to the browser. If a method does not return a status
        // or boolean indicating the action succeeded or failed, a generic exception is also 
        // thrown by the caller.
        //

        private void WriteToEventLog(Exception e, string action)
        {
            var log = new EventLog { Source = EVENT_SOURCE, Log = EVENT_LOG };

            string message = "An exception occurred communicating with the data source.\n\n";
            message += "Action: " + action + "\n\n";
            message += "Exception: " + e;

            log.WriteEntry(message);
        }

    }
}
