using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Serialization;

namespace WindAuth.Models.LoffTim.Login
{

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "", IsNullable = false)]
    public partial class RESPONSE
    {

        private RESPONSERESULT rESULTField;

        private RESPONSECONTENT cONTENTField;

        private string tRACKINGSERVERField;

        private bool nETWORK_CONNECTEDField;

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

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("TRACKING-SERVER")]
        public string TRACKINGSERVER
        {
            get
            {
                return this.tRACKINGSERVERField;
            }
            set
            {
                this.tRACKINGSERVERField = value;
            }
        }

        /// <remarks/>
        public bool NETWORK_CONNECTED
        {
            get
            {
                return this.nETWORK_CONNECTEDField;
            }
            set
            {
                this.nETWORK_CONNECTEDField = value;
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

        private RESPONSECONTENTCONFIG cONFIGField;

        /// <remarks/>
        public RESPONSECONTENTCONFIG CONFIG
        {
            get
            {
                return this.cONFIGField;
            }
            set
            {
                this.cONFIGField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class RESPONSECONTENTCONFIG
    {

        private RESPONSECONTENTCONFIGRUNTIME rUNTIMEField;

        private RESPONSECONTENTCONFIGVIEW vIEWField;

        private RESPONSECONTENTCONFIGCACHE cACHEField;

        /// <remarks/>
        public RESPONSECONTENTCONFIGRUNTIME RUNTIME
        {
            get
            {
                return this.rUNTIMEField;
            }
            set
            {
                this.rUNTIMEField = value;
            }
        }

        /// <remarks/>
        public RESPONSECONTENTCONFIGVIEW VIEW
        {
            get
            {
                return this.vIEWField;
            }
            set
            {
                this.vIEWField = value;
            }
        }

        /// <remarks/>
        public RESPONSECONTENTCONFIGCACHE CACHE
        {
            get
            {
                return this.cACHEField;
            }
            set
            {
                this.cACHEField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class RESPONSECONTENTCONFIGRUNTIME
    {

        private RESPONSECONTENTCONFIGRUNTIMECUSTOMER cUSTOMERField;

        private RESPONSECONTENTCONFIGRUNTIMESIMLIST sIMLISTField;

        private RESPONSECONTENTCONFIGRUNTIMEINTERFACELIST iNTERFACELISTField;

        /// <remarks/>
        public RESPONSECONTENTCONFIGRUNTIMECUSTOMER CUSTOMER
        {
            get
            {
                return this.cUSTOMERField;
            }
            set
            {
                this.cUSTOMERField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("SIM-LIST")]
        public RESPONSECONTENTCONFIGRUNTIMESIMLIST SIMLIST
        {
            get
            {
                return this.sIMLISTField;
            }
            set
            {
                this.sIMLISTField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("INTERFACE-LIST")]
        public RESPONSECONTENTCONFIGRUNTIMEINTERFACELIST INTERFACELIST
        {
            get
            {
                return this.iNTERFACELISTField;
            }
            set
            {
                this.iNTERFACELISTField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class RESPONSECONTENTCONFIGRUNTIMECUSTOMER
    {

        private uint msisdnField;

        private string profileField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public uint msisdn
        {
            get
            {
                return this.msisdnField;
            }
            set
            {
                this.msisdnField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string profile
        {
            get
            {
                return this.profileField;
            }
            set
            {
                this.profileField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class RESPONSECONTENTCONFIGRUNTIMESIMLIST
    {

        private byte counterField;

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
    public partial class RESPONSECONTENTCONFIGRUNTIMEINTERFACELIST
    {

        private RESPONSECONTENTCONFIGRUNTIMEINTERFACELISTINTERFACE[] iNTERFACEField;

        private byte counterField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("INTERFACE")]
        public RESPONSECONTENTCONFIGRUNTIMEINTERFACELISTINTERFACE[] INTERFACE
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
    public partial class RESPONSECONTENTCONFIGRUNTIMEINTERFACELISTINTERFACE
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

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class RESPONSECONTENTCONFIGVIEW
    {

        private RESPONSECONTENTCONFIGVIEWHEADER hEADERField;

        private RESPONSECONTENTCONFIGVIEWADVERTISING aDVERTISINGField;

        /// <remarks/>
        public RESPONSECONTENTCONFIGVIEWHEADER HEADER
        {
            get
            {
                return this.hEADERField;
            }
            set
            {
                this.hEADERField = value;
            }
        }

        /// <remarks/>
        public RESPONSECONTENTCONFIGVIEWADVERTISING ADVERTISING
        {
            get
            {
                return this.aDVERTISINGField;
            }
            set
            {
                this.aDVERTISINGField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class RESPONSECONTENTCONFIGVIEWHEADER
    {

        private RESPONSECONTENTCONFIGVIEWHEADERLOGO lOGOField;

        /// <remarks/>
        public RESPONSECONTENTCONFIGVIEWHEADERLOGO LOGO
        {
            get
            {
                return this.lOGOField;
            }
            set
            {
                this.lOGOField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class RESPONSECONTENTCONFIGVIEWHEADERLOGO
    {

        private string sectionField;

        private string urlField;

        private bool externalField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string section
        {
            get
            {
                return this.sectionField;
            }
            set
            {
                this.sectionField = value;
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
        public bool external
        {
            get
            {
                return this.externalField;
            }
            set
            {
                this.externalField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class RESPONSECONTENTCONFIGVIEWADVERTISING
    {

        private RESPONSECONTENTCONFIGVIEWADVERTISINGBANNER bANNERField;

        /// <remarks/>
        public RESPONSECONTENTCONFIGVIEWADVERTISINGBANNER BANNER
        {
            get
            {
                return this.bANNERField;
            }
            set
            {
                this.bANNERField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class RESPONSECONTENTCONFIGVIEWADVERTISINGBANNER
    {

        private string sectionField;

        private string imgField;

        private string urlField;

        private bool externalField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string section
        {
            get
            {
                return this.sectionField;
            }
            set
            {
                this.sectionField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string img
        {
            get
            {
                return this.imgField;
            }
            set
            {
                this.imgField = value;
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
        public bool external
        {
            get
            {
                return this.externalField;
            }
            set
            {
                this.externalField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class RESPONSECONTENTCONFIGCACHE
    {

        private string nameField;

        private byte expirationField;

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
        public byte expiration
        {
            get
            {
                return this.expirationField;
            }
            set
            {
                this.expirationField = value;
            }
        }
    }


}