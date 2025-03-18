using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Entities
{
    public class EmployeeCafe
    {
        public Guid CafeID { get; private set; }

        public string EmployeeID { get; private set; }

        public EmployeeCafe(Guid cafeID, string employeeID)
        {
            CafeID = cafeID;
            EmployeeID = employeeID;
        }

        public void Update(Guid cafeID, string employeeID)
        {
            CafeID = cafeID;
            EmployeeID = employeeID;
        }
    }    
}
