using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DB_AngoraLib.Models
{
    public class Breeder : User
    {
        public string BreederRegNo { get; set; }
        public BreederBrand BreederBrand { get; set; }

        public virtual ICollection<Rabbit> RabbitsOwned { get; set; }
        public virtual ICollection<Rabbit> RabbitsLinked { get; set; }

        public virtual ICollection<TransferRequst> RabbitTransfers_Issued { get; set; }
        public virtual ICollection<TransferRequst> RabbitTransfers_Received { get; set; }

        public Breeder(string breederRegNo, string firstName, string lastName, string roadName, int zipCode, string city, string email, string phone, string password)
           : base(firstName, lastName, roadName, zipCode, city, email, phone, password)
        {
            BreederRegNo = breederRegNo;
            InitializeCollections();
        }

        public Breeder()
        {
            InitializeCollections();
        }

        private void InitializeCollections()
        {
            RabbitsOwned = new List<Rabbit>();
            RabbitsLinked = new List<Rabbit>();
            RabbitTransfers_Issued = new List<TransferRequst>();
            RabbitTransfers_Received = new List<TransferRequst>();
        }
    }
}
