using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WindAuth.Models.LoffTim.Info
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

        private byte codeField;

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
        public byte code
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

        private RESPONSECONTENTINFO iNFOField;

        /// <remarks/>
        public RESPONSECONTENTINFO INFO
        {
            get
            {
                return this.iNFOField;
            }
            set
            {
                this.iNFOField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class RESPONSECONTENTINFO
    {

        private RESPONSECONTENTINFOPROFILE pROFILEField;

        private RESPONSECONTENTINFOPROMOTIONS pROMOTIONSField;

        private RESPONSECONTENTINFOLOYALTY lOYALTYField;

        /// <remarks/>
        public RESPONSECONTENTINFOPROFILE PROFILE
        {
            get
            {
                return this.pROFILEField;
            }
            set
            {
                this.pROFILEField = value;
            }
        }

        /// <remarks/>
        public RESPONSECONTENTINFOPROMOTIONS PROMOTIONS
        {
            get
            {
                return this.pROMOTIONSField;
            }
            set
            {
                this.pROMOTIONSField = value;
            }
        }

        /// <remarks/>
        public RESPONSECONTENTINFOLOYALTY LOYALTY
        {
            get
            {
                return this.lOYALTYField;
            }
            set
            {
                this.lOYALTYField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class RESPONSECONTENTINFOPROFILE
    {

        private RESPONSECONTENTINFOPROFILECONTRACT cONTRACTField;

        private RESPONSECONTENTINFOPROFILEPREPAID pREPAIDField;

        private string idField;

        private string nAMEField;

        private string typeField;

        private bool availableField;

        private string updateField;

        private string labelField;

        /// <remarks/>
        public RESPONSECONTENTINFOPROFILEPREPAID PREPAID
        {
            get
            {
                return this.pREPAIDField;
            }
            set
            {
                this.pREPAIDField = value;
            }
        }

        public RESPONSECONTENTINFOPROFILECONTRACT CONTRACT
        {
            get
            {
                return this.cONTRACTField;
            }
            set
            {
                this.cONTRACTField = value;
            }
        }

        /// <remarks/>
        public string ID
        {
            get
            {
                return this.idField;
            }
            set
            {
                this.idField = value;
            }
        }

        /// <remarks/>
        public string NAME
        {
            get
            {
                return this.nAMEField;
            }
            set
            {
                this.nAMEField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string type
        {
            get
            {
                return this.typeField;
            }
            set
            {
                this.typeField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public bool available
        {
            get
            {
                return this.availableField;
            }
            set
            {
                this.availableField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string update
        {
            get
            {
                return this.updateField;
            }
            set
            {
                this.updateField = value;
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
    public partial class RESPONSECONTENTINFOPROFILEPREPAID
    {

        private RESPONSECONTENTINFOPROFILEPREPAIDCREDIT cREDITField;

        private RESPONSECONTENTINFOPROFILEPREPAIDSIM_EXPIRATION sIM_EXPIRATIONField;

        /// <remarks/>
        public RESPONSECONTENTINFOPROFILEPREPAIDCREDIT CREDIT
        {
            get
            {
                return this.cREDITField;
            }
            set
            {
                this.cREDITField = value;
            }
        }

        /// <remarks/>
        public RESPONSECONTENTINFOPROFILEPREPAIDSIM_EXPIRATION SIM_EXPIRATION
        {
            get
            {
                return this.sIM_EXPIRATIONField;
            }
            set
            {
                this.sIM_EXPIRATIONField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class RESPONSECONTENTINFOPROFILEPREPAIDCREDIT
    {

        private RESPONSECONTENTINFOPROFILEPREPAIDCREDITBONUS bONUSField;

        private RESPONSECONTENTINFOPROFILEPREPAIDCREDITLASTEVENT lASTEVENTField;

        private decimal valueField;

        private string updateField;

        private string labelField;

        /// <remarks/>
        public RESPONSECONTENTINFOPROFILEPREPAIDCREDITBONUS BONUS
        {
            get
            {
                return this.bONUSField;
            }
            set
            {
                this.bONUSField = value;
            }
        }

        /// <remarks/>
        public RESPONSECONTENTINFOPROFILEPREPAIDCREDITLASTEVENT LASTEVENT
        {
            get
            {
                return this.lASTEVENTField;
            }
            set
            {
                this.lASTEVENTField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public decimal value
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

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string update
        {
            get
            {
                return this.updateField;
            }
            set
            {
                this.updateField = value;
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
    public partial class RESPONSECONTENTINFOPROFILEPREPAIDCREDITBONUS
    {

        private string typeField;

        private decimal valueField;

        private string labelField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string type
        {
            get
            {
                return this.typeField;
            }
            set
            {
                this.typeField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public decimal value
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
    public partial class RESPONSECONTENTINFOPROFILEPREPAIDCREDITLASTEVENT
    {

        private string valueField;

        private string labelField;

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
    public partial class RESPONSECONTENTINFOPROFILEPREPAIDSIM_EXPIRATION
    {

        private string dateField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string date
        {
            get
            {
                return this.dateField;
            }
            set
            {
                this.dateField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class RESPONSECONTENTINFOPROMOTIONS
    {

        private object iNFO_MSGField;

        private RESPONSECONTENTINFOPROMOTIONSINTERFACELIST iNTERFACELISTField;

        private bool availableField;

        private string updateField;

        private string labelField;

        /// <remarks/>
        public object INFO_MSG
        {
            get
            {
                return this.iNFO_MSGField;
            }
            set
            {
                this.iNFO_MSGField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("INTERFACE-LIST")]
        public RESPONSECONTENTINFOPROMOTIONSINTERFACELIST INTERFACELIST
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

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public bool available
        {
            get
            {
                return this.availableField;
            }
            set
            {
                this.availableField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string update
        {
            get
            {
                return this.updateField;
            }
            set
            {
                this.updateField = value;
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
    public partial class RESPONSECONTENTINFOPROMOTIONSINTERFACELIST
    {

        private RESPONSECONTENTINFOPROMOTIONSINTERFACELISTINTERFACE[] iNTERFACEField;

        private byte counterField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("INTERFACE")]
        public RESPONSECONTENTINFOPROMOTIONSINTERFACELISTINTERFACE[] INTERFACE
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
    public partial class RESPONSECONTENTINFOPROMOTIONSINTERFACELISTINTERFACE
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
    public partial class RESPONSECONTENTINFOLOYALTY
    {

        private RESPONSECONTENTINFOLOYALTYPLAN pLANField;

        /// <remarks/>
        public RESPONSECONTENTINFOLOYALTYPLAN PLAN
        {
            get
            {
                return this.pLANField;
            }
            set
            {
                this.pLANField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class RESPONSECONTENTINFOLOYALTYPLAN
    {

        private bool availableField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public bool available
        {
            get
            {
                return this.availableField;
            }
            set
            {
                this.availableField = value;
            }
        }
    }

}

/// <remarks/>
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
public partial class RESPONSECONTENTINFOPROFILECONTRACT
{

    private RESPONSECONTENTINFOPROFILECONTRACTBALANCE bALANCEField;

    /// <remarks/>
    public RESPONSECONTENTINFOPROFILECONTRACTBALANCE BALANCE
    {
        get
        {
            return this.bALANCEField;
        }
        set
        {
            this.bALANCEField = value;
        }
    }
}

/// <remarks/>
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
public partial class RESPONSECONTENTINFOPROFILECONTRACTBALANCE
{

    private decimal valueField;

    private string updateField;

    private string labelField;

    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public decimal value
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

    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string update
    {
        get
        {
            return this.updateField;
        }
        set
        {
            this.updateField = value;
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
