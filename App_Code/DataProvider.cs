

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


        public abstract int AddItem(int moduleId, string title, string description, string thumbUrl, string imageUrl, int viewOrder, bool autoGenerateThumb);
        public abstract void UpdateItem(int itemId, int moduleId, string title, string description, string thumbUrl, string imageUrl, int viewOrder, bool autoGenerateThumb, int imageWidth, int imageHeight, int thumbWidth, int thumbHeight, long lastWriteTime);
        public abstract void UpdateItemOrder(int itemId, int viewOrder);
        public abstract IDataReader GetItems(int moduleId);
        public abstract IDataReader GetItemById(int itemId);
        public abstract void DeleteItem(int itemId);


        public abstract void AddActivation(string activationCode, string registrationCode, string host, string productCode, bool IsPrimary, string baseActivationCode);
        public abstract IDataReader GetActivations(string productCode, string host);
        public abstract IDataReader GetAllActivations(string productCode);


    }
}
