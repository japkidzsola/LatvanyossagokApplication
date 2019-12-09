using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LatvanyossagokApplication
{
    public partial class Form1 : Form
    {
        MySqlConnection conn;

        public Form1()
        {
            InitializeComponent();
            conn = new MySqlConnection("Server=localhost; Port=3307; Database=latvanyossagokdb; Uid=root; Pwd=;");
            conn.Open();

            VarosListazas();
            
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
        void VarosListazas()
        {
            listBoxVaros.Items.Clear();

            var cmd = conn.CreateCommand();
            cmd.CommandText = @"SELECT id, nev, lakossag
                                FROM varosok
                                ORDER BY id";
            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    var id = reader.GetInt32("id");
                    var nev = reader.GetString("nev");
                    var lakossag = reader.GetInt32("lakossag");
                    var varos = new Varos(id, nev, lakossag);
                    listBoxVaros.Items.Add(varos);
                }
            }
        }

        void LatvanyossagListazas()
        {
            if (listBoxVaros.SelectedIndex == -1)
            {
                return;
            }
            else
            {
                listBoxLatvanyossag.Items.Clear();
                var varos = (Varos)listBoxVaros.SelectedItem;
                var varosId = varos.Id;
                var cmd = conn.CreateCommand();
                cmd.CommandText = @"SELECT id, nev, leiras, ar, varos_id
                                FROM latvanyossagok
                                WHERE varos_id = @id";
                cmd.Parameters.AddWithValue("@id", varosId);
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var id = reader.GetInt32("id");
                        var nev = reader.GetString("nev");
                        var leiras = reader.GetString("leiras");
                        var ar = reader.GetInt32("ar");
                        var varos_id = reader.GetInt32("varos_id");
                        var latvanyossag = new Latvanyossag(id, nev, leiras, ar, varos_id);
                        listBoxLatvanyossag.Items.Add(latvanyossag);
                    }
                }
            }

        }

        private void buttonVarosHozzaad_Click(object sender, EventArgs e)
        {
            var cmd = conn.CreateCommand();
            cmd.CommandText = @"INSERT INTO varosok ( nev, lakossag)
                                VALUES (@nev, @lakossag)";
            cmd.Parameters.AddWithValue("@nev", textBoxVarosNev.Text);
            cmd.Parameters.AddWithValue("@lakossag", textBoxLakossag.Text);

            cmd.ExecuteNonQuery();
            VarosListazas();
        }

        private void buttonLatvanyossagHozzaad_Click(object sender, EventArgs e)
        {
            var varos = (Varos)listBoxVaros.SelectedItem;
            var id = varos.Id;
            var cmd = conn.CreateCommand();
            cmd.CommandText = @"INSERT INTO latvanyossagok (nev, leiras, ar, varos_id)
                                VALUES (@nev, @leiras, @ar, @varos_id)";
            cmd.Parameters.AddWithValue("@nev", textBoxLatvanyossagNev.Text);
            cmd.Parameters.AddWithValue("@leiras", textBoxLeiras.Text);
            cmd.Parameters.AddWithValue("@ar", textBoxAr.Text);
            cmd.Parameters.AddWithValue("@varos_id", id);

            cmd.ExecuteNonQuery();
            LatvanyossagListazas();
        }

        private void buttonVarosTorles_Click(object sender, EventArgs e)
        {
            var varos =(Varos)listBoxVaros.SelectedItem;
            var id = varos.Id;
            var cmd = conn.CreateCommand();
            cmd.CommandText = "DELETE FROM latvanyossagok WHERE varos_id = @id";
            cmd.Parameters.AddWithValue("@id", id);
            cmd.ExecuteNonQuery();

            var cmdDelete = conn.CreateCommand();
            cmdDelete.CommandText = "DELETE FROM varosok WHERE id = @id";
            cmdDelete.Parameters.AddWithValue("@id", id);
            cmdDelete.ExecuteNonQuery();
            VarosListazas();
            LatvanyossagListazas();
        }

        private void buttonLatvanyossagTorles_Click(object sender, EventArgs e)
        {
            var latvanyossag = (Latvanyossag)listBoxLatvanyossag.SelectedItem;
            var id = latvanyossag.Id;
            var cmdDelete = conn.CreateCommand();
            cmdDelete.CommandText = "DELETE FROM latvanyossagok WHERE id = @id";
            cmdDelete.Parameters.AddWithValue("@id", id);
            cmdDelete.ExecuteNonQuery();
            //VarosListazas();
            LatvanyossagListazas();
        }

        private void listBoxVaros_SelectedIndexChanged(object sender, EventArgs e)
        {
            LatvanyossagListazas();
        }
    }
}
