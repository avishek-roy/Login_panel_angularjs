using BOEService.Entites.BOE;
using BOEService.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BOEService.Factories
{
    public class CommonFactory : GenericFactory<BOEEntities, TBLB_COMPANY>
    {
        public override bool IsDuplicate(TBLB_COMPANY entity)
        {
            return this.FindBy(d => d.ID != entity.ID && d.Code == entity.Code).Any();
        }
    }

    public class UrlFactory : GenericFactory<BOEEntities, TBLA_URL>
    {

    }

  

    public class CommonFactorys : ICommonFactory
        {

            public class Result
            {
                public string Message { get; set; }
                public bool IsSucess { get; set; }

            }

            private BOEEntities context;
            private IGenericFactory<TBLB_COMPANY> factory;

            public CommonFactorys()
            {
                this.context = new BOEEntities();
            }

            //public CommonFactorys(BackOfficeExtendedEntities context)
            //{
            //    this.context = context;
            //}

            //public CommonFactorys(string clientType, string balance, string clints)
            //{
            //    this.context = new BackOfficeExtendedEntities();
            //}

            //public CommonFactorys(List<tblCompany> tblIpoApplication)
            //{
            //    this.context = new BackOfficeExtendedEntities();
            //}

            //public CommonFactorys(string clients, decimal ipo_declaration_id, Guid makeby)
            //{
            //    this.context = new BackOfficeExtendedEntities();
            //}




    }
}
