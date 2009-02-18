
using System;
using DotNetNuke;
using System.Data;
using System.Web.UI.WebControls;

using DotNetNuke.Framework;

namespace avt.FastShot
{
    public abstract class DataProvider
    {
        // singleton reference to the instantiated object 
        private static DataProvider objProvider = null;

        // constructor
        static DataProvider()
        {
            CreateProvider();
        }

        // dynamically create provider
        private static void CreateProvider()
        {
            objProvider = (DataProvider)Reflection.CreateObject("data", "avt.FastShot", "");
        }

        // return the provider
        public static DataProvider Instance()
        {
            return objProvider;
        }


        public abstract int AddItem(int moduleId, string title, string description, string thumbUrl, string imageUrl, int viewOrder);
        public abstract void UpdateItem(int itemId, int moduleId, string title, string description, string thumbUrl, string imageUrl, int viewOrder);
        public abstract IDataReader GetItems(int moduleId);
        public abstract IDataReader GetItemById(int itemId);
        public abstract void DeleteItem(int itemId);

        //public abstract IDataReader GetProduct(Guid productGuid);
        //public abstract IDataReader GetProducts(int portalId);
        //public abstract IDataReader GetProductByName(int portalId, string productName);
        //public abstract IDataReader GetProductVersion(Guid versionGuid);
        //public abstract IDataReader GetProductVersions(Guid productGuid);
        //public abstract IDataReader GetProductVersionByName(Guid productGuid, string version);

        //public abstract IDataReader GetLicense(Guid licenseGuid);
        //public abstract IDataReader GetLicenseByKey(string regCode);
        //public abstract IDataReader GetLicenseType(Guid licenseTypeGuid);
        //public abstract IDataReader GetLicenseTypeByName(Guid versionGuid, string licenseTypeName);

        //public abstract Guid CreateLicense(Guid productGuid, Guid licenseTypeGuid);
        //public abstract void SetRegistrationCode(Guid licenseId, string regCode);

        //public abstract IDataReader GetActivation(Guid activationGuid);
        //public abstract Guid AddActivation(Guid licenseGuid, string actKey, int portalId, string installationKey, string aliases, string hostname);
        //public abstract IDataReader GetActivationByCode(string regCode, string activationCode);
        //public abstract IDataReader GetActivationBySpec(Guid licenseGuid, string hostname);

        //public abstract void SetOwner(Guid licenseGuid, int userId);

        //public abstract void SetInstanceData(string instanceId, string instanceDataName, string instanceDataValue);
        //public abstract string GetInstanceData(string instanceId, string instanceDataName);
        
        //public abstract int CreateProfile(int portalID, string profileName, string profileDescription);
        //public abstract void UpdateProfile(int profileId, string profileName, string profileDescription);
        //public abstract void DeleteProfile(int profileId);

        //public abstract IDataReader GetProfileById(int profileId);
        //public abstract IDataReader GetProfiles(int portalId);
        
        //public abstract int BindProfile(string instanceLinkageId, int profileId);
        //public abstract int UnbindProfile(string instanceLinkageId, int profileId);

        //public abstract void IncreaseProfilePrecedence(string instanceLinkageId, int profileId);
        //public abstract void DecreaseProfilePrecedence(string instanceLinkageId, int profileId);
        
        //public abstract IDataReader GetInstanceProfiles(string instanceLinkageId);
        //public abstract IDataReader GetProfilesNotInInstance(string instanceLinkageId);

        //public abstract void SetProfileData(int profileId, string profileDataName, string profileDataValue);
        //public abstract string GetProfileData(int profileId, string profileDataName);

        //#region Profile Items

        //public abstract int AddProfileItem(int profileId, int itemId, int itemType, int parentItemId, int order, int level, string caption, string target);
        //public abstract void RemoveProfileItems(int profileId);
        //public abstract IDataReader GetProfileItems(int profileId);

        //#endregion


        //#region Profile Roles

        //public abstract int AddProfileRole(int profileId, int roleId);
        //public abstract void RemoveProfileRole(int profileRoleId);
        //public abstract void RemoveProfileRole(int profileId, int roleId);
        //public abstract IDataReader GetProfileRoles(int profileId);

        //#endregion



    }
}
