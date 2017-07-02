using System.Collections.Generic;

namespace zakupki_ftp.schemas
{
    internal class Notification
    {
        public Notice notice;
        public List<Documents> documents;
        public Documents oneDocument;
        public List<Lots> lots;
        public Lots oneLot;
        public List<Products> products;
        public Products oneProduct;

        public struct Notice
        {
            public string notificationNumber;
            public string notificationType;
            public string orderName;
            public string publishDate;
            public string href;
            public string placingWay_code;
            public string orgName;
            public string orgRegNum;
            public string orgINN;
            public string orgOGRN;
            public string placementFeature_code;
            public string placementFeature_name;
            public string modificationDate;
            public string modificationDescription;
            public string ep_name;
            public string ep_url;
            public string status;
            public string purchaseMethodCode;
            public string purchaseMethodName;
            public string submissionCloseDateTime;
            public string region; // b2b
            public string okdpPath; // b2b
        }

        // информация о прикрепленных документах
        public struct Documents
        {
            public string fileName;
            public string docDescription;
            public string url;

            public Documents(string fn,string dd, string ur)
            {
                fileName = fn;
                docDescription = dd;
                url = ur;
            }
        }

        // информация о лотах
        public struct Lots
        {
            public string subject;
            public string currencyCode;
            public string currencyName;
            public string maxPrice;

        }

        // информация о продуктах лота
        public struct Products
        {
            public string code;
            public string name;
            public int lotID;
            public string price;
            public string fullname;
        }

        public Notification()
        {
            notice = new Notice();
            documents = new List<Documents>();
            oneDocument = new Documents();
            lots = new List<Lots>();
            oneLot = new Lots();
            products = new List<Products>();
            oneProduct = new Products();
        }
    }
}
