using MNPZ.DAO;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MNPZ
{
    public class OperationEx
    {
        public OperationEx() { }
        
        OperationContext opContext = new OperationContext();
        public void SaveOperation(Operation op) 
        {
            var insertOp = opContext.InsertOperation(op);
            if (insertOp.IsError)
            MessageBox.Show(insertOp.Message);
        }
    }
}
