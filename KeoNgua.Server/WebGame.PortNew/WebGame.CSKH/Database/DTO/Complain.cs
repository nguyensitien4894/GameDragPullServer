using System;

namespace MsWebGame.CSKH.Database.DTO
{
    public class Complain
    {
        public int SearchType { get; set; }
      
        public long ComplainID { get; set; }
   
        public int ComplainType { get; set; }
   
        public string Title { get; set; }
       
        public string Content { get; set; }
   
        public long UserID { get; set; }

        public string UserName { get; set; }
 
        public string UserImage { get; set; }

        public string UserExplain { get; set; }
      
        public string UserProcessResult { get; set; }
      
        public long DefendantID { get; set; }

        public string DefendantName { get; set; }
      
        public string DefendantImage { get; set; }
      
        public string DefendantExplain { get; set; }
   
        public string DefendantProcessResult { get; set; }

        public string Result { get; set; }
   
        public int ComplainStatus { get; set; }
      
        public DateTime CreateDate { get; set; }
      
        public DateTime UpdateDate { get; set; }
     
        public long TransID { get; set; }
      
        public string TransCode { get; set; }
      
        public DateTime TransDate { get; set; }
       
        public int TranStatus { get; set; }
       
        public int RequestStatus { get; set; }

        public string CreateUserName { get; set; }
        public string CreatePhoneContact { get; set; }
        public string CreatePhoneOtp { get; set; }
        public int CreateUserTotalComplainCnt { get; set; }
        public int CreateUserTranBuyCnt { get; set; }
        public int CreateUserTranSellCnt { get; set; }
        public string CreateUserDisplayName { get; set; }

        public string ReceiveUserName { get; set; }
        public string ReceiveUserDisplayName { get; set; }
        public string ReceivePhoneContact { get; set; }
        public string ReceivePhoneOtp { get; set; }
        public int  ReceiverTotalComplainCnt { get; set; }
        public int ReceiverTranBuyCnt { get; set; }
        public int ReceiverTranSellCnt { get; set; }

        public long ReceiveUserID { get; set; }
        public long CreateUserID { get; set; }

        public string ProcessCall { get; set; }
    }
}