using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PSSL_Environment
{

    public class Interpreter
    {
        // Instance of the interpreter to make
        //  it a singleton
        private static Interpreter m_instance;

        private Interpreter() { }

        // Get instance to the interpreter class
        public static Interpreter GetInstance()
        {

            if(m_instance == null)
            {
                m_instance = new Interpreter(); 
            }
            return m_instance;
   
        }
    }
}
