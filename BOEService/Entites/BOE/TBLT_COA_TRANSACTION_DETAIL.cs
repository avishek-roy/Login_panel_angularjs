//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace BOEService.Entites.BOE
{
    using System;
    using System.Collections.Generic;
    
    public partial class TBLT_COA_TRANSACTION_DETAIL
    {
        public int COATRANSACTIONDETAILID { get; set; }
        public int COATransactionID { get; set; }
        public string GLACNO { get; set; }
        public Nullable<decimal> DEBITAMT { get; set; }
        public Nullable<decimal> CREDITAMT { get; set; }
        public string TransType { get; set; }
        public string CHQNO { get; set; }
        public string REMARKS { get; set; }
        public Nullable<int> CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<int> ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
    }
}
