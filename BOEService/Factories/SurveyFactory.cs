using BOEService.Entites.BOE;
using BOEService.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BOEService.Factories
{
    public class DepositeFactory : GenericFactory<BOEEntities, TBLT_SURVEY_DEPOSIT>
    {

    }

    public class ComnucationFactory : GenericFactory<BOEEntities, TBLT_SURVEY_COMMUNICATION>
    {

    }


    public class SurveyFactorys : ISurveyFactory
    {

        public class Result
        {
            public string message { get; set; }
            public bool isSucess { get; set; }

        }

        private BOEEntities context;
        private IGenericFactory<TBLT_SURVEY_DEPOSIT> survayDepositeFactory;
        private IGenericFactory<TBLT_SURVEY_COMMUNICATION> comnucationFactory;
    

        public SurveyFactorys()
        {
            this.context = new BOEEntities();
        }

        public SurveyFactorys(BOEEntities context)
        {
            this.context = context;
        }
      
       



    }                                                                                                           

}
