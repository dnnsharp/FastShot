
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


        public override int AddItem(int moduleId, string title, string description, string thumbUrl, string imageUrl, int viewOrder, bool autoGenerateThumb, string tplParams)
        {
            return Convert.ToInt32(SqlHelper.ExecuteScalar(ConnectionString, DatabaseOwner + ObjectQualifier + "avtFastShot_InsertItem", moduleId, title, description, thumbUrl, imageUrl, viewOrder, autoGenerateThumb, tplParams));
        }

        public override void UpdateItem(int itemId, int moduleId, string title, string description, string thumbUrl, string imageUrl, int viewOrder, bool autoGenerateThumb, int imageWidth, int imageHeight, int thumbWidth, int thumbHeight, long lastWriteTime, string tplParams)
        {
            SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner + ObjectQualifier + "avtFastShot_UpdateItem", itemId, moduleId, title, description, thumbUrl, imageUrl, viewOrder, autoGenerateThumb, imageWidth, imageHeight, thumbWidth, thumbHeight, lastWriteTime, tplParams);
        }

        public override void UpdateItemOrder(int itemId, int viewOrder)
        {
            SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner + ObjectQualifier + "avtFastShot_UpdateItemOrder", itemId, viewOrder);
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

        #endregion

    }
}
