
using System;
using System.Data;
using System.Data.SqlClient;
using Microsoft.ApplicationBlocks.Data;

using DotNetNuke;
using DotNetNuke.Common.Utilities;


namespace avt.FastShot
{
    public class SqlDataProvider : DataProvider
    {

        #region "Private Members"

        private const string ProviderType = "data";

        private DotNetNuke.Framework.Providers.ProviderConfiguration _providerConfiguration = DotNetNuke.Framework.Providers.ProviderConfiguration.GetProviderConfiguration(ProviderType);
        private string _connectionString;
        private string _providerPath;
        private string _objectQualifier;
        private string _databaseOwner;

        #endregion

        #region "Constructors"

        public SqlDataProvider()
        {

            // Read the configuration specific information for this provider
            DotNetNuke.Framework.Providers.Provider objProvider = (DotNetNuke.Framework.Providers.Provider)_providerConfiguration.Providers[_providerConfiguration.DefaultProvider];

            // Read the attributes for this provider
            //Get Connection string from web.config
            _connectionString = DotNetNuke.Common.Utilities.Config.GetConnectionString();

            if (_connectionString == "") {
                // Use connection string specified in provider
                _connectionString = objProvider.Attributes["connectionString"];
            }

            _providerPath = objProvider.Attributes["providerPath"];

            _objectQualifier = objProvider.Attributes["objectQualifier"];
            if (_objectQualifier != "" & _objectQualifier.EndsWith("_") == false) {
                _objectQualifier += "_";
            }

            _databaseOwner = objProvider.Attributes["databaseOwner"];
            if (_databaseOwner != "" & _databaseOwner.EndsWith(".") == false) {
                _databaseOwner += ".";
            }
        }

        #endregion

        #region "Properties"

        public string ConnectionString
        {
            get { return _connectionString; }
        }

        public string ProviderPath
        {
            get { return _providerPath; }
        }

        public string ObjectQualifier
        {
            get { return _objectQualifier; }
        }

        public string DatabaseOwner
        {
            get { return _databaseOwner; }
        }

        #endregion

        #region Public Methods


        public override int AddItem(int moduleId, string title, string description, string thumbUrl, string imageUrl, int viewOrder)
        {
            SqlHelper.ExecuteScalar(ConnectionString, DatabaseOwner + ObjectQualifier + "avtFastShot_InsertItem", moduleId, title, description, thumbUrl, imageUrl, viewOrder);
            return 0;
        }

        public override void UpdateItem(int itemId, int moduleId, string title, string description, string thumbUrl, string imageUrl, int viewOrder)
        {
            SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner + ObjectQualifier + "avtFastShot_UpdateItem", itemId, moduleId, title, description, thumbUrl, imageUrl, viewOrder);
        }

        public override IDataReader GetItems(int moduleId)
        {
            return SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "avtFastShot_GetItems", moduleId);
        }

        public override IDataReader GetItemById(int itemId)
        {
            return SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "avtFastShot_GetItemById", itemId);
        }

        public override void DeleteItem(int itemId)
        {
            SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner + ObjectQualifier + "avtFastShot_DeleteItem", itemId);
        }

        //public override IDataReader GetProduct(Guid productGuid)
        //{
        //    return SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "avtRegCore_GetProduct", productGuid);
        //}

        //public override IDataReader GetProducts(int portalId)
        //{
        //    return SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "avtRegCore_GetProducts", portalId);
        //}

        //public override IDataReader GetProductByName(int portalId, string productName)
        //{
        //    return SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "avtRegCore_GetProductByName", portalId, productName);
        //}

        //public override IDataReader GetProductVersion(Guid versionGuid)
        //{
        //    return SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "avtRegCore_GetProductVersion", versionGuid);
        //}

        //public override IDataReader GetProductVersions(Guid productGuid)
        //{
        //    return SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "avtRegCore_GetProductVersions", productGuid);
        //}

        //public override IDataReader GetProductVersionByName(Guid productGuid, string version)
        //{
        //    return SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "avtRegCore_GetProductVersionByName", productGuid, version);
        //}

        //public override IDataReader GetLicense(Guid licenseGuid)
        //{
        //    return SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "avtRegCore_GetLicense", licenseGuid);
        //}

        //public override IDataReader GetLicenseByKey(string regCode)
        //{
        //    return SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "avtRegCore_GetLicenseByKey", regCode);
        //}

        //public override IDataReader GetLicenseType(Guid licenseTypeGuid)
        //{
        //    return SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "avtRegCore_GetLicenseType", licenseTypeGuid);
        //}

        //public override IDataReader GetLicenseTypeByName(Guid versionGuid, string licenseTypeName)
        //{
        //    return SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "avtRegCore_GetLicenseTypeByName", versionGuid, licenseTypeName);
        //}

        //public override Guid CreateLicense(Guid productGuid, Guid licenseTypeGuid)
        //{
        //    //try {
        //        return (Guid) SqlHelper.ExecuteScalar(ConnectionString, DatabaseOwner + ObjectQualifier + "avtRegCore_CreateLicense", productGuid, licenseTypeGuid);
        //    //} catch (Exception) {
        //    //    return Null.NullGuid;
        //    //}
        //}

        //public override void SetRegistrationCode(Guid licenseId, string regCode)
        //{
        //    SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner + ObjectQualifier + "avtRegCore_SetRegistrationCode", licenseId, regCode);
        //}

        //public override IDataReader GetActivation(Guid activationGuid)
        //{
        //    return SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "avtRegCore_GetActivation", activationGuid);
        //}

        //public override Guid AddActivation(Guid licenseGuid, string actKey, int portalId, string installationKey, string aliases, string hostname)
        //{
        //    return (Guid)SqlHelper.ExecuteScalar(ConnectionString, DatabaseOwner + ObjectQualifier + "avtRegCore_AddActivation", licenseGuid, actKey, installationKey, portalId, aliases, hostname);
        //}

        //public override IDataReader GetActivationByCode(string regCode, string activationCode)
        //{
        //    return SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "avtRegCore_GetActivationByCode", regCode, activationCode);
        //}

        //public override IDataReader GetActivationBySpec(Guid licenseGuid, string hostname)
        //{
        //    return SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "avtRegCore_GetActivationBySpec", licenseGuid, hostname);
        //}

        //public override void SetOwner(Guid licenseGuid, int userId)
        //{
        //    SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner + ObjectQualifier + "avtRegCore_SetOwner", licenseGuid, userId); 
        //}

        ////private object GetNull(object Field)
        ////{
        ////    return DotNetNuke.Common.Utilities.Null.GetNull(Field, DBNull.Value);
        ////}


        ////public override void SetInstanceData(string instanceId, string instanceDataName, string instanceDataValue)
        ////{
        ////    SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner + ObjectQualifier + "avtNavXp_SetInstanceData", instanceId, instanceDataName, instanceDataValue);
        ////}

        ////public override string GetInstanceData(string instanceId, string instanceDataName)
        ////{
        ////    object data = SqlHelper.ExecuteScalar(ConnectionString, DatabaseOwner + ObjectQualifier + "avtNavXp_GetInstanceData", instanceId, instanceDataName);
        ////    if (data != null) {
        ////        return Convert.ToString(data);
        ////    }
        ////    return null;
        ////}

        ////public override int CreateProfile(int portalID, string profileName, string profileDescription)
        ////{
        ////    try {
        ////        return Convert.ToInt32(SqlHelper.ExecuteScalar(ConnectionString, DatabaseOwner + ObjectQualifier + "avtNavXp_CreateProfile", portalID, profileName, profileDescription));
        ////    } catch (Exception e) {
        ////        throw new NavXpException(ErrorCode.DuplicateKey, "A profile with the same name already exists.", "profile");
        ////    }
        ////}

        ////public override void UpdateProfile(int profileId, string profileName, string profileDescription)
        ////{
        ////    SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner + ObjectQualifier + "avtNavXp_UpdateProfile", profileId, profileName, profileDescription);
        ////}

        ////public override void DeleteProfile(int profileId)
        ////{
        ////    SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner + ObjectQualifier + "avtNavXp_DeleteProfile", profileId);
        ////}

        ////public override IDataReader GetProfileById(int profileId)
        ////{
        ////    return SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "avtNavXp_GetProfileById", profileId);
        ////}

        ////public override IDataReader GetProfiles(int portalId)
        ////{
        ////    return SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "avtNavXp_GetProfiles", portalId);
        ////}

        ////public override int BindProfile(string instanceLinkageId, int profileId)
        ////{
        ////    try {
        ////        return Convert.ToInt32(SqlHelper.ExecuteScalar(ConnectionString, DatabaseOwner + ObjectQualifier + "avtNavXp_BindProfile", instanceLinkageId, profileId));
        ////    } catch (Exception) {
        ////        return -1;
        ////    }
        ////}

        ////public override int UnbindProfile(string instanceLinkageId, int profileId)
        ////{
        ////    try {
        ////        return Convert.ToInt32(SqlHelper.ExecuteScalar(ConnectionString, DatabaseOwner + ObjectQualifier + "avtNavXp_UnbindProfile", instanceLinkageId, profileId));
        ////    } catch (Exception) {
        ////        return -1;
        ////    }
        ////}

        ////public override void IncreaseProfilePrecedence(string instanceLinkageId, int profileId)
        ////{
        ////    SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner + ObjectQualifier + "avtNavXp_IncreaseProfilePrecedence", instanceLinkageId, profileId);
        ////}

        ////public override void DecreaseProfilePrecedence(string instanceLinkageId, int profileId)
        ////{
        ////    SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner + ObjectQualifier + "avtNavXp_DecreaseProfilePrecedence", instanceLinkageId, profileId);
        ////}

        ////public override IDataReader GetInstanceProfiles(string instanceLinkageId)
        ////{
        ////    return SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "avtNavXp_GetInstanceProfiles", instanceLinkageId);
        ////}

        ////public override IDataReader GetProfilesNotInInstance(string instanceLinkageId)
        ////{
        ////    return SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "avtNavXp_GetProfilesNotInInstance", instanceLinkageId);
        ////}

        ////public override void SetProfileData(int profileId, string profileDataName, string profileDataValue)
        ////{
        ////    SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner + ObjectQualifier + "avtNavXp_SetProfileData", profileId, profileDataName, profileDataValue);
        ////}

        ////public override string GetProfileData(int profileId, string profileDataName)
        ////{
        ////    return Convert.ToString(SqlHelper.ExecuteScalar(ConnectionString, DatabaseOwner + ObjectQualifier + "avtNavXp_GetProfileData", profileId, profileDataName));
        ////}


        ////#region Profile Items

        ////public override int AddProfileItem(int profileId, int itemId, int itemType, int parentItemId, int order, int level, string caption, string target)
        ////{
        ////    try {
        ////        return Convert.ToInt32(SqlHelper.ExecuteScalar(ConnectionString, DatabaseOwner + ObjectQualifier + "avtNavXp_AddProfileItem", profileId, itemId, itemType, parentItemId, order, level, caption, target));
        ////    } catch (Exception) {
        ////        return -1;
        ////    }
        ////}

        ////public override void RemoveProfileItems(int profileId)
        ////{
        ////    SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner + ObjectQualifier + "avtNavXp_RemoveProfileItems", profileId);
        ////}

        ////public override IDataReader GetProfileItems(int profileId)
        ////{
        ////    return SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "avtNavXp_GetProfileItems", profileId);
        ////}

        ////#endregion


        ////#region Profile Roles

        ////public override int AddProfileRole(int profileId, int roleId)
        ////{
        ////    try {
        ////        return Convert.ToInt32(SqlHelper.ExecuteScalar(ConnectionString, DatabaseOwner + ObjectQualifier + "avtNavXp_AddProfileRole", profileId, roleId));
        ////    } catch (Exception) {
        ////        return -1;
        ////    }
        ////}

        ////public override void RemoveProfileRole(int profileRoleId)
        ////{
        ////    SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner + ObjectQualifier + "avtNavXp_RemoveProfileRole", profileRoleId);
        ////}

        ////public override void RemoveProfileRole(int profileId, int roleId)
        ////{
        ////    SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner + ObjectQualifier + "avtNavXp_RemoveProfileRoleEx", profileId, roleId);
        ////}

        ////public override IDataReader GetProfileRoles(int profileId)
        ////{
        ////    return SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "avtNavXp_GetProfileRoles", profileId);
        ////}
        

        ////#endregion


        #endregion

    }
}
