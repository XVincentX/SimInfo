using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WindAuth.Models.LoffTim.InterfaceData
{

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "", IsNullable = false)]
    public partial class RESPONSE
    {

        private RESPONSERESULT rESULTField;

        private RESPONSECONTENT cONTENTField;

        /// <remarks/>
        public RESPONSERESULT RESULT
        {
            get
            {
                return this.rESULTField;
            }
            set
            {
                this.rESULTField = value;
            }
        }

        /// <remarks/>
        public RESPONSECONTENT CONTENT
        {
            get
            {
                return this.cONTENTField;
            }
            set
            {
                this.cONTENTField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class RESPONSERESULT
    {

        private string mESSAGEField;

        private int codeField;

        /// <remarks/>
        public string MESSAGE
        {
            get
            {
                return this.mESSAGEField;
            }
            set
            {
                this.mESSAGEField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public int code
        {
            get
            {
                return this.codeField;
            }
            set
            {
                this.codeField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class RESPONSECONTENT
    {

        private RESPONSECONTENTPROMODETAIL pROMODETAILField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("PROMO-DETAIL")]
        public RESPONSECONTENTPROMODETAIL PROMODETAIL
        {
            get
            {
                return this.pROMODETAILField;
            }
            set
            {
                this.pROMODETAILField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class RESPONSECONTENTPROMODETAIL
    {

        private RESPONSECONTENTPROMODETAILPROMOTION pROMOTIONField;

        private RESPONSECONTENTPROMODETAILPROMO_DESC pROMO_DESCField;

        private RESPONSECONTENTPROMODETAILACTIVATION_DATE aCTIVATION_DATEField;

        private RESPONSECONTENTPROMODETAILEXPIRATION_DATE eXPIRATION_DATEField;

        private RESPONSECONTENTPROMODETAILSTATUS sTATUSField;

        private RESPONSECONTENTPROMODETAILDESCRIPTIONS dESCRIPTIONSField;

        private RESPONSECONTENTPROMODETAILINTERFACE iNTERFACEField;

        /// <remarks/>
        public RESPONSECONTENTPROMODETAILPROMOTION PROMOTION
        {
            get
            {
                return this.pROMOTIONField;
            }
            set
            {
                this.pROMOTIONField = value;
            }
        }

        /// <remarks/>
        public RESPONSECONTENTPROMODETAILPROMO_DESC PROMO_DESC
        {
            get
            {
                return this.pROMO_DESCField;
            }
            set
            {
                this.pROMO_DESCField = value;
            }
        }

        /// <remarks/>
        public RESPONSECONTENTPROMODETAILACTIVATION_DATE ACTIVATION_DATE
        {
            get
            {
                return this.aCTIVATION_DATEField;
            }
            set
            {
                this.aCTIVATION_DATEField = value;
            }
        }

        /// <remarks/>
        public RESPONSECONTENTPROMODETAILEXPIRATION_DATE EXPIRATION_DATE
        {
            get
            {
                return this.eXPIRATION_DATEField;
            }
            set
            {
                this.eXPIRATION_DATEField = value;
            }
        }

        /// <remarks/>
        public RESPONSECONTENTPROMODETAILSTATUS STATUS
        {
            get
            {
                return this.sTATUSField;
            }
            set
            {
                this.sTATUSField = value;
            }
        }

        /// <remarks/>
        public RESPONSECONTENTPROMODETAILDESCRIPTIONS DESCRIPTIONS
        {
            get
            {
                return this.dESCRIPTIONSField;
            }
            set
            {
                this.dESCRIPTIONSField = value;
            }
        }

        /// <remarks/>
        public RESPONSECONTENTPROMODETAILINTERFACE INTERFACE
        {
            get
            {
                return this.iNTERFACEField;
            }
            set
            {
                this.iNTERFACEField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class RESPONSECONTENTPROMODETAILPROMOTION
    {

        private string nameField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string name
        {
            get
            {
                return this.nameField;
            }
            set
            {
                this.nameField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class RESPONSECONTENTPROMODETAILPROMO_DESC
    {

        private string valueField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string value
        {
            get
            {
                return this.valueField;
            }
            set
            {
                this.valueField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class RESPONSECONTENTPROMODETAILACTIVATION_DATE
    {

        private string valueField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string value
        {
            get
            {
                return this.valueField;
            }
            set
            {
                this.valueField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class RESPONSECONTENTPROMODETAILEXPIRATION_DATE
    {

        private string valueField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string value
        {
            get
            {
                return this.valueField;
            }
            set
            {
                this.valueField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class RESPONSECONTENTPROMODETAILSTATUS
    {

        private string valueField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string value
        {
            get
            {
                return this.valueField;
            }
            set
            {
                this.valueField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class RESPONSECONTENTPROMODETAILDESCRIPTIONS
    {

        private RESPONSECONTENTPROMODETAILDESCRIPTIONSDESCRIPTION[] dESCRIPTIONField;

        private byte counterField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("DESCRIPTION")]
        public RESPONSECONTENTPROMODETAILDESCRIPTIONSDESCRIPTION[] DESCRIPTION
        {
            get
            {
                return this.dESCRIPTIONField;
            }
            set
            {
                this.dESCRIPTIONField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public byte counter
        {
            get
            {
                return this.counterField;
            }
            set
            {
                this.counterField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class RESPONSECONTENTPROMODETAILDESCRIPTIONSDESCRIPTION
    {

        private string valueField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string value
        {
            get
            {
                return this.valueField;
            }
            set
            {
                this.valueField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class RESPONSECONTENTPROMODETAILINTERFACE
    {

        private string nameField;

        private string urlField;

        private bool enabledField;

        private string labelField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string name
        {
            get
            {
                return this.nameField;
            }
            set
            {
                this.nameField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string url
        {
            get
            {
                return this.urlField;
            }
            set
            {
                this.urlField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public bool enabled
        {
            get
            {
                return this.enabledField;
            }
            set
            {
                this.enabledField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string label
        {
            get
            {
                return this.labelField;
            }
            set
            {
                this.labelField = value;
            }
        }
    }


}