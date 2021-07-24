using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Data.SQLite;

namespace Kasboekje
{
    public partial class Form1 : Form
    {
        private const string defaultDB = "default.db";

        public Form1()
        {
            InitializeComponent();

            // check for SQLite databases
            /*            string[] files = Directory.GetFiles(Directory.GetCurrentDirectory(), "*.db");

                        foreach(string file in files)
                        {
                            filesList.Items.Add(file);
                        }*/

            // check if we already have a default database
            bool hasDefaultDB = File.Exists(Directory.GetCurrentDirectory() + $@"\{defaultDB}");

            if (hasDefaultDB)
            {
                // make sure we have the tables as well
                if (!SQLiteLib.checkForDefaultTables(defaultDB))
                {
                    // we have a database but no tables
                    throw new InvalidDataException();
                }
            }
            else
            {
                // create the default tables
                if (!SQLiteLib.createTables(defaultDB))
                {
                    // failed to write tables to file at this point in time
                    throw new InvalidOperationException();
                }
            }                
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}
