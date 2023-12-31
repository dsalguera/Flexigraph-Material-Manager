﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;
using static Flexigraph_Software_Manager.Form1;

namespace Flexigraph_Software_Manager
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
            dataGridView1.DefaultCellStyle.SelectionBackColor = Color.White;
            dataGridView1.DefaultCellStyle.SelectionForeColor = Color.Black;
            Captura_Fecha();
            label1.Text = fecha;
            LeerAuto();
            LeerContrasena();

            LeerSoloNombres();
            Bloquear();


            //DataCheckBoxs
            //dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.DisplayedCells;
            
            
            checkData.HeaderText = "";
            checkData.Name = "Botones";
            dataGridView1.Columns.Insert(0, checkData);

            dataGridView1.Columns[0].FillWeight = 15;

        }

        AutoCompleteStringCollection DataCollection = new AutoCompleteStringCollection();
        DataGridViewButtonColumn checkData = new DataGridViewButtonColumn();

        void Desbloquear()
        {
            groupBox6.Enabled = true;
            groupBox7.Enabled = true;
            groupBox8.Enabled = true;
            dataGridView3.Enabled = true;
            dataGridView3.Visible = true;
        }

        void Bloquear()
        {
            groupBox6.Enabled = false;
            groupBox7.Enabled = false;
            groupBox8.Enabled = false;
            dataGridView3.Enabled = false;
            dataGridView3.Visible = false;
            textBox2.Text = "";
        }

        private void dataGridView1_Enter(object sender, EventArgs e)
        {

        }

        void SumarIngresos()
        {
            val = 0;

            for (int i = 0; i < dataGridView1.RowCount - 1; i++)
            {

                try
                {
                    //Condicion para ingresos
                    if (dataGridView1.Rows[i].Cells["Ingresos"].Value.ToString().Equals(""))
                    {
                        dataGridView1.Rows[i].Cells["Ingresos"].Value = 0;
                    }
                    else
                    {
                        val += Double.Parse(dataGridView1.Rows[i].Cells["Ingresos"].Value.ToString());
                    }
                }
                catch (Exception)
                {
                }

            }

            //label4.Text = "Total de Entradas: ¢0.00";
            label4.Text = "Total de Entradas: ¢" + String.Format("{0:n}", val);
        }


        void SumarEgresos()
        {
            val2 = 0;

            for (int i = 0; i < dataGridView2.RowCount - 1; i++)
            {

                //Condicion para ingresos
                if (dataGridView2.Rows[i].Cells["Egresos"].Value.ToString().Equals(""))
                {
                    dataGridView2.Rows[i].Cells["Egresos"].Value = 0;
                }
                else
                {
                    val2 += Double.Parse(dataGridView2.Rows[i].Cells["Egresos"].Value.ToString());
                }

            }

            label5.Text = "Total de Salidas: ¢" + String.Format("{0:n}", val2);
            
        }

        double TotalIngresos = 0;
        double val = 0;
        double val2 = 0;
        double bloquados = 0;

        private void dataGridView1_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            string sobrescribir = "";
            if (e.RowIndex >= 0)
            {
                try
                {
                    val = 0;
                    bloquados = 0;

                    for (int i = 0; i < dataGridView1.RowCount - 1; i++)
                    {

                        //Condicion para ingresos
                        if (dataGridView1.Rows[i].Cells["Ingresos"].Value.ToString().Equals(""))
                        {
                            dataGridView1.Rows[i].Cells["Ingresos"].Value = 0;
                        }
                        else
                        {
                            val += Double.Parse(dataGridView1.Rows[i].Cells["Ingresos"].Value.ToString());
                            
                        }


                       
                    }
                    
                    //label4.Text = "Total de Entradas: ¢0.00";

                    sobrescribir = dataGridView1.Rows[e.RowIndex].Cells["Descripción"].Value.ToString();
                    sobrescribir = sobrescribir + " -> Hora: " + label2.Text;
                    dataGridView1.Rows[e.RowIndex].Cells["Descripción"].Value = sobrescribir;

                    label4.Text = "Total de Entradas: ¢" + String.Format("{0:n}", val);
                    
                }
                catch (Exception ex)
                {

                }
                
            }

            
        }

        String fecha = "";
        String soloTxtFecha = "";
        String hora = "", minutos = "", segundos = "";

        private void button4_Click(object sender, EventArgs e)
        {

        }

        public void Guardar()
        {
            try
            {
                Eliminar(monthCalendar1.SelectionRange.Start.ToShortDateString(),dirListaMonto,"ListaIngreso");
                Eliminar(monthCalendar1.SelectionRange.Start.ToShortDateString(), dirListaMontoEg, "ListaEgreso");
                AgregarLista(monthCalendar1.SelectionRange.Start.ToShortDateString());
                AgregarListaEgreso(monthCalendar1.SelectionRange.Start.ToShortDateString());
                MessageBox.Show("Se han guardado todos los registros!", "Operación realizada");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message+"\n\n"+ex.StackTrace);
            }
            
        }

        String dirListaMonto = Directory.GetCurrentDirectory().ToString() + "\\ListaMontoIngresos.xml";
        String dirListaMontoEg = Directory.GetCurrentDirectory().ToString() + "\\ListaMontoEgresos.xml";
        String dirListaBitacora = Directory.GetCurrentDirectory().ToString() + "\\ListaMontoBitacora.xml";

        void AgregarLista(String nombreAtt)
        {

            doc.Load(dirListaMonto);
            string strNamespace = doc.DocumentElement.NamespaceURI;
            XmlNode ListaIngreso = doc.CreateNode(XmlNodeType.Element, "ListaIngreso", strNamespace);
            XmlAttribute attr2 = doc.CreateAttribute("fecha");

            attr2.Value = nombreAtt;

            XmlNode Ingresos = doc.CreateNode(XmlNodeType.Element, "Ingresos", null);


            XmlNode[] Lista = new XmlNode[dataGridView1.Rows.Count - 1];

            //Variables
            XmlNode Monto, Descripcion;

            for (int i = 0; i < dataGridView1.Rows.Count - 1; i++)
            {
                Lista[i] = doc.CreateNode(XmlNodeType.Element, "Lista", null);
                Monto = doc.CreateNode(XmlNodeType.Element, "Monto", strNamespace);
                Descripcion = doc.CreateNode(XmlNodeType.Element, "Descripcion", strNamespace);

                Monto.InnerText = "" + dataGridView1.Rows[i].Cells["Ingresos"].Value;
                Descripcion.InnerText = "" + dataGridView1.Rows[i].Cells["Descripción"].Value;

                Lista[i].AppendChild(Monto);
                Lista[i].AppendChild(Descripcion);
                Ingresos.AppendChild(Lista[i]);

            }


            ListaIngreso.Attributes.Append(attr2);
            ListaIngreso.AppendChild(Ingresos);
            doc.DocumentElement.AppendChild(ListaIngreso);
            doc.Save(dirListaMonto);

            dataGridView1.Rows.Clear();

        }

        void AgregarListaEgreso(String nombreAtt)
        {

            doc.Load(dirListaMontoEg);
            string strNamespace = doc.DocumentElement.NamespaceURI;
            XmlNode ListaEgreso = doc.CreateNode(XmlNodeType.Element, "ListaEgreso", strNamespace);
            XmlAttribute attr2 = doc.CreateAttribute("fecha");

            attr2.Value = nombreAtt;

            XmlNode Egresos = doc.CreateNode(XmlNodeType.Element, "Egresos", null);


            XmlNode[] Lista = new XmlNode[dataGridView2.Rows.Count - 1];

            //Variables
            XmlNode Monto, Descripcion;

            for (int i = 0; i < dataGridView2.Rows.Count - 1; i++)
            {
                Lista[i] = doc.CreateNode(XmlNodeType.Element, "Lista", null);
                Monto = doc.CreateNode(XmlNodeType.Element, "Monto", strNamespace);
                Descripcion = doc.CreateNode(XmlNodeType.Element, "Descripcion", strNamespace);

                Monto.InnerText = "" + dataGridView2.Rows[i].Cells["Egresos"].Value;
                Descripcion.InnerText = "" + dataGridView2.Rows[i].Cells["Descripción2"].Value;

                Lista[i].AppendChild(Monto);
                Lista[i].AppendChild(Descripcion);
                Egresos.AppendChild(Lista[i]);

            }


            ListaEgreso.Attributes.Append(attr2);

            ListaEgreso.AppendChild(Egresos);
            

            doc.DocumentElement.AppendChild(ListaEgreso);

            //////////////////////
            doc.Save(dirListaMontoEg);

            dataGridView2.Rows.Clear();
            //label106.Text = "¢00.000";
            //textBox19.Text = "";
            //label96.Text = "¢00.000";


        }

        void LeerDatosIngresos(String nombreC)
        {
            dataGridView1.Rows.Clear();

            doc.Load(dirListaMonto);
            String nombre = nombreC;

            foreach (XmlNode nodo in doc.SelectNodes("ListaIngreso/ListaIngreso[@fecha='" + nombre + "']"))
            {

                foreach (XmlNode nodoN in doc.SelectNodes("ListaIngreso/ListaIngreso[@fecha='" + nombre + "']/Ingresos/Lista"))
                {
                    String descripcion = nodoN.SelectSingleNode("Descripcion").InnerText;
                    String monto = nodoN.SelectSingleNode("Monto").InnerText;

                    dataGridView1.Rows.Add("",monto, descripcion);
                }

            }


            //label106.Text = "¢" + String.Format("{0:n}", Double.Parse(monto));
            //textBox19.Text = "" + efectivo;
            //label96.Text = "¢" + String.Format("{0:n}", Double.Parse(cambio));


        }

        void LeerDatosBitacora(String nombreC)
        {
            richTextBox2.Clear();

            doc.Load(dirListaBitacora);
            String nombre = nombreC;

            foreach (XmlNode nodo in doc.SelectNodes("ListaIngreso/ListaBitacora[@fecha='" + nombre + "']"))
            {

                foreach (XmlNode nodoN in doc.SelectNodes("ListaIngreso/ListaBitacora[@fecha='" + nombre + "']/Bitacora"))
                {
                    String descripcion = nodoN.SelectSingleNode("Descripcion").InnerText;


                    richTextBox2.Text = descripcion;
                }

            }


            //label106.Text = "¢" + String.Format("{0:n}", Double.Parse(monto));
            //textBox19.Text = "" + efectivo;
            //label96.Text = "¢" + String.Format("{0:n}", Double.Parse(cambio));


        }

        void LeerDatosEgresos(String nombreC)
        {
            dataGridView2.Rows.Clear();

            doc.Load(dirListaMontoEg);
            String nombre = nombreC;

            foreach (XmlNode nodo in doc.SelectNodes("ListaEgreso/ListaEgreso[@fecha='" + nombre + "']"))
            {

                foreach (XmlNode nodoN in doc.SelectNodes("ListaEgreso/ListaEgreso[@fecha='" + nombre + "']/Egresos/Lista"))
                {
                    String descripcion = nodoN.SelectSingleNode("Descripcion").InnerText;
                    String monto = nodoN.SelectSingleNode("Monto").InnerText;

                    dataGridView2.Rows.Add(monto, descripcion);
                }

            }


            //label106.Text = "¢" + String.Format("{0:n}", Double.Parse(monto));
            //textBox19.Text = "" + efectivo;
            //label96.Text = "¢" + String.Format("{0:n}", Double.Parse(cambio));


        }

        private void button3_Click(object sender, EventArgs e)
        {
            Guardar();
            GuardarBitacora(monthCalendar1.SelectionRange.Start.ToShortDateString());
            MostrarDate();
            
        }

        void GuardarBitacora(String nombreAtt)
        {
            
            doc.Load(dirListaBitacora);
            string strNamespace = doc.DocumentElement.NamespaceURI;
            XmlNode ListaIngreso = doc.CreateNode(XmlNodeType.Element, "ListaBitacora", strNamespace);
            XmlAttribute attr2 = doc.CreateAttribute("fecha");

            attr2.Value = nombreAtt;

            XmlNode Bitacora = doc.CreateNode(XmlNodeType.Element, "Bitacora", null);
            
            //Variables
            XmlNode Descripcion;
            Descripcion = doc.CreateNode(XmlNodeType.Element, "Descripcion", strNamespace);
            Descripcion.InnerText = richTextBox2.Text;

            Bitacora.AppendChild(Descripcion);

            
            ListaIngreso.Attributes.Append(attr2);
            ListaIngreso.AppendChild(Bitacora);

            doc.DocumentElement.AppendChild(ListaIngreso);
            doc.Save(dirListaBitacora);

            richTextBox2.Clear();

            LeerDatosBitacora(nombreAtt);

        }

        private void monthCalendar1_DateSelected(object sender, DateRangeEventArgs e)
        {
            MostrarDate();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            CapturaPantalla("Contro de Gastos");
        }

        private static Bitmap bmp;
        private static Graphics gfxScreenshot;
        string path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

        void CapturaPantalla(string tipo)
        {
            try
            {
                //this.BringToFront();
                //this.TopMost = true;
                //this.Focus();

                var frm = Form2.ActiveForm;
                //Bitmap bmp = new Bitmap(frm.Width, frm.Height, PixelFormat.Format32bppArgb);
                //frm.DrawToBitmap(bmp, new Rectangle(0, 0, bmp.Width, bmp.Height));
                //bmp.Save(path + "\\Cotizacion de " + textBox19.Text + " " + fecha + ".png");

                //Metodo 2
                bmp = new Bitmap(frm.Bounds.Width, frm.Bounds.Height, PixelFormat.Format32bppArgb);
                // Create a graphics object from the bitmap
                gfxScreenshot = Graphics.FromImage(bmp);
                // Take the screenshot from the upper left corner to the right bottom corner
                gfxScreenshot.CopyFromScreen(frm.Bounds.X, frm.Bounds.Y, 0, 0, frm.Size, CopyPixelOperation.SourceCopy);
                // Save the screenshot to the specified path that the user has chosen

                Captura_Fecha();

                bmp.Save(path + "\\"+tipo+" Flexigraph " + soloTxtFecha + ".jpeg", ImageFormat.Jpeg);
                String dir = path + "\\"+tipo+" Flexigraph " + soloTxtFecha + ".jpeg";

                MessageBox.Show("Se ha guardado la captura de pantalla en " + dir, "Captura de pantalla");

                string argument = "/select, \"" + dir + "\"";

                ProcessStartInfo info = new ProcessStartInfo();
                info.FileName = "explorer";
                info.Arguments = argument;
                Process.Start(info);
            }
            catch (Exception)
            {
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            int hh = DateTime.Now.Hour;
            int mm = DateTime.Now.Minute;
            int sec = DateTime.Now.Second;

            string time = "";

            if (hh < 10)
            {
                time += "0" + hh;
            }
            else
            {
                time += hh;
            }

            time += ":";

            if (mm < 10)
            {
                time += "0" + mm;
            }
            else
            {
                time += mm;
            }

            time += ":";
            // agregando los segundos

            if (sec < 10)
            {
                time += "0" + sec;
            }
            else
            {
                time += sec;
            }

            label2.Text = time;
        }


        private void Form2_Load(object sender, EventArgs e)
        {
            this.ControlBox = false;
            LeerCaja();
            MostrarDate();
            
            SumarIngresos();
            label4.BackColor = Color.Green;
            label14.BackColor = Color.Gray;
            label5.BackColor = Color.Red;

            label14.ForeColor = Color.White;
            label4.ForeColor = Color.White;
            label5.ForeColor = Color.White;

        }



        private void Form2_FormClosed(object sender, FormClosedEventArgs e)
        {

        }

        double horax = 0;
        double minutosx = 0;

        private void button2_Click(object sender, EventArgs e)
        {
            GuardarAuto();
            LeerAuto();
        }
        String dir = Directory.GetCurrentDirectory().ToString() + "\\datosAuto.xml";
        XmlDocument doc = new XmlDocument();

        void LeerAuto()
        {
            int hora = 0;
            int minuto = 0;
            doc.Load(dir);
            string text = null;

            foreach (XmlNode node in doc.DocumentElement.ChildNodes)
            {
                text = node.InnerText; //or loop through its children as well
            }

            String horaTot = text;
            label6.Text = horaTot + ":00";

            String supHora = horaTot.Substring(0, 2).ToString();
            String supMin = horaTot.Substring(3).ToString();

            hora = Int32.Parse(supHora);
            minuto = Int32.Parse(supMin);

            numericUpDown1.Value = hora;
            numericUpDown2.Value = minuto;

        }

        void GuardarAuto()
        {
            XmlWriter writer = XmlWriter.Create(dir);
            writer.WriteStartDocument();
            writer.WriteStartElement("Datos");

            //root2
            writer.WriteStartElement("Dato");

            String horass = "", minutoss = "";

            if (EsMenor(Int32.Parse(numericUpDown1.Value.ToString())) == true)
            {
                horass = "0" + numericUpDown1.Value.ToString();
            }
            else
            {
                horass = numericUpDown1.Value.ToString();
            }

            if (EsMenor(Int32.Parse(numericUpDown2.Value.ToString())) == true)
            {
                minutoss = "0" + numericUpDown2.Value.ToString();
            }
            else
            {
                minutoss = numericUpDown2.Value.ToString();
            }

            writer.WriteElementString("Hora", horass + ":" + minutoss);

            writer.WriteEndElement();

            writer.WriteEndElement();
            writer.WriteEndDocument();
            writer.Flush();

            writer.Close();

            MessageBox.Show("Se ha guardado la hora.", "Guardado");
        }

        bool EsMenor(int numero)
        {
            if (numero < 10)
            {
                return true;
            }
            return false;
        }

        System.Timers.Timer aTimer = new System.Timers.Timer();

        private void label2_TextChanged(object sender, EventArgs e)
        {
            if (label2.Text.ToString().Equals(label6.Text.ToString()))
            {
                this.Visible = true;
                
                aTimer.Elapsed += new ElapsedEventHandler(OnTimedEvent);
                aTimer.Interval = 1500;
                aTimer.Enabled = true;
                
            }
        }

        void OnTimedEvent(object source, ElapsedEventArgs e)
        {
            CapturaPantalla("Contro de Gastos");
            aTimer.Stop();
            //Guardar();
        }

        void Captura_Fecha()
        {
            DateTime fechaHoy = DateTime.Now;
            fecha = "" + fechaHoy.DayOfWeek + ", " + fechaHoy.Day + "/" + fechaHoy.Month + "/" + fechaHoy.Year;
            soloTxtFecha = fechaHoy.Day + "-" + fechaHoy.Month + "-" + fechaHoy.Year;
            int h = fechaHoy.Hour;
            int m = fechaHoy.Minute;
            int s = fechaHoy.Second;

            hora = "" + h;
            minutos = "" + m;
            segundos = "" + s;
        }

        String item3 = "";

        private void dataGridView1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                try
                {
                    dataGridView1.Rows.Add();
                }
                catch (Exception)
                {
                }
            }
        }

        private void dataGridView2_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            string sobrescribir = "";
            if (e.RowIndex >= 0)
            {
                try
                {
                    val2 = 0;

                    for (int i = 0; i < dataGridView2.RowCount - 1; i++)
                    {

                        //Condicion para ingresos
                        if (dataGridView2.Rows[i].Cells["Egresos"].Value.ToString().Equals(""))
                        {
                            dataGridView2.Rows[i].Cells["Egresos"].Value = 0;
                        }
                        else
                        {
                            val2 += Double.Parse(dataGridView2.Rows[i].Cells["Egresos"].Value.ToString());
                        }

                    }

                    //label4.Text = "Total de Entradas: ¢0.00";
                    label5.Text = "Total de Salidas: ¢" + String.Format("{0:n}", val2);

                    sobrescribir = dataGridView2.Rows[e.RowIndex].Cells["Descripción2"].Value.ToString();
                    sobrescribir = sobrescribir + " -> Hora: " + label2.Text;
                    dataGridView2.Rows[e.RowIndex].Cells["Descripción2"].Value = sobrescribir;

                }
                catch (Exception ex)
                {
                   
                }

            }
        }

        private void label4_TextChanged(object sender, EventArgs e)
        {
            Resta();
        }

        void Resta()
        {
            double total = val - val2;
            label3.Text = "Total de Entradas y Salidas: ¢" + String.Format("{0:n}", total);
            if (total > 0)
            {
                label3.BackColor = Color.Green;
                label3.ForeColor = Color.White;
            }
            else if (total < 0)
            {
                label3.BackColor = Color.Red;
                label3.ForeColor = Color.White;
            }

            
        }

        private void button4_Click_1(object sender, EventArgs e)
        {
            
        }

        private void label5_TextChanged(object sender, EventArgs e)
        {
            Resta();
        }

        private void dataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            Aviso(sender,e,dataGridView1);
        }

        void Aviso(object sender, DataGridViewCellEventArgs e, DataGridView dataGridView)
        {
            DialogResult dr = MessageBox.Show("¿Esta seguro que desea eliminar el siguiente registro?",
                      "Confirmar Eliminación", MessageBoxButtons.YesNo);
            switch (dr)
            {
                case DialogResult.Yes:
                    EliminarEntrada(dataGridView, e.RowIndex);
                    SumarIngresos();
                    SumarEgresos();
                    break;
                case DialogResult.No:
                    break;
            }
        }

        void EliminarEntrada(DataGridView dataGridView, int fila)
        {

            try
            {
                dataGridView.Rows.Remove(dataGridView.Rows[fila]);
            }
            catch (Exception)
            {
            }
        }

        private void dataGridView2_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            Aviso(sender, e, dataGridView2);
        }
        
        private void button4_Click_2(object sender, EventArgs e)
        {
            this.Visible = false;
            //Guardar();
        }

        private void button5_Click_1(object sender, EventArgs e)
        {
            GuardarCaja();
            LeerCaja();
        }

        String dirCajaMonto = Directory.GetCurrentDirectory().ToString() + "\\datosMontoCaja.xml";

        void GuardarCaja()
        {
            XmlWriter writer = XmlWriter.Create(dirCajaMonto);
            writer.WriteStartDocument();
            writer.WriteStartElement("Datos");

            //root2
            writer.WriteStartElement("Dato");
            writer.WriteElementString("Caja", textBox1.Text.ToString());

            writer.WriteEndElement();

            writer.WriteEndElement();
            writer.WriteEndDocument();
            writer.Flush();

            writer.Close();

            MessageBox.Show("Se ha guardado el registro de caja", "Guardado");
        }

        void LeerCaja()
        {
            double caja = 0;
            doc.Load(dirCajaMonto);
            string text = null;

            foreach (XmlNode node in doc.DocumentElement.ChildNodes)
            {
                text = node.InnerText; //or loop through its children as well
            }
            caja = Double.Parse(text);
            textBox1.Text = "" + caja;

            dataGridView1.Rows.Add("",caja, "Fondo de caja chica");
            
        }

        private void label3_TextChanged(object sender, EventArgs e)
        {
            if (true)
            {

            }
        }

        private void groupBox5_Enter(object sender, EventArgs e)
        {

        }

        private void button4_Click_3(object sender, EventArgs e)
        {
            this.Visible = false;
        }

        private void button5_Click_2(object sender, EventArgs e)
        {

        }

        private void button6_Click(object sender, EventArgs e)
        {
            Verificar_Bloqueo();
        }


        String contrasenaFile = "";

        void LeerContrasena ()
        {
            double imp = 0;
            doc.Load(dirContrasena);
            string text = null;

            foreach (XmlNode node in doc.DocumentElement.ChildNodes)
            {
                text = node.InnerText; //or loop through its children as well
            }
            contrasenaFile = text;
            

        }

        String dirContrasena = Directory.GetCurrentDirectory().ToString() + "\\pass.xml";

        void Verificar_Bloqueo()
        {
            if (textBox2.Text.ToString().Equals(contrasenaFile))
            {
                Desbloquear();
            }
            else
            {
                MessageBox.Show("¡Contraseña no válida!","Error de Entrada");
                Bloquear();
            }
        }

        private void button8_Click(object sender, EventArgs e)
        {
            EliminarRegistroPrestamo(textBox3.Text, dirListaPrestamo, "ListaPrestamo");
        }

        private void button10_Click(object sender, EventArgs e)
        {
            try
            {
                EliminarRegistro(textBox3.Text, dirListaPrestamo, "ListaPrestamo");
                GuardarCliente(textBox3.Text);
                LeerDatosPrestamo(textBox3.Text);
                MessageBox.Show("Se ha guardado el registro!", "Exito al guardar");
                Recarga();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.StackTrace);
            }
        }

        void LeerSoloNombres()
        {
            doc.Load(dirListaPrestamo);

            foreach (XmlNode node in doc.SelectNodes("ListaPrestamo/ListaPrestamo"))
            {
                try
                {
                    textBox3.AutoCompleteCustomSource.Add(node.SelectSingleNode("NombrePersona").InnerText.ToString());
                    listView1.Items.Add(node.SelectSingleNode("NombrePersona").InnerText.ToString());
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }

            doc.Save(dirListaPrestamo);

        }

        void EliminarRegistroPrestamo(String nombreAtt, String dir, String descendants)
        {

            var xDoc = XDocument.Load(dir);

            foreach (var elem in xDoc.Document.Descendants(descendants))
            {
                foreach (var attr in elem.Attributes("nombre"))
                {
                    if (attr.Value.Equals(nombreAtt))
                        elem.RemoveAll();
                }
            }

            xDoc.Save(dir);
            //MessageBox.Show("Deleted Successfully");
            LimpiarCampos();
            MessageBox.Show("Se ha eliminado el registro!", "Exito al eliminar");

        }

        void LimpiarCampos()
        {
            textBox3.Text = "";
            textBox4.Text = "";
            textBox5.Text = "";
            label12.Text = "000.000";
            label16.Text = "000.000";
            richTextBox1.Text = "";
            dataGridView3.Rows.Clear();
        }

        void LeerDatosPrestamo(String nombreC)
        {
            dataGridView3.Rows.Clear();

            doc.Load(dirListaPrestamo);
            String nombre = nombreC;

            foreach (XmlNode nodo in doc.SelectNodes("ListaPrestamo/ListaPrestamo[@nombre='" + nombre + "']"))
            {
                // Comentario por que habia error
                try
                {
                    String prestamo = nodo.SelectSingleNode("Prestamo").InnerText;
                    String telefono = nodo.SelectSingleNode("Telefono").InnerText;
                    String nombrePers = nodo.SelectSingleNode("NombrePersona").InnerText;
                    textBox4.Text = "" + prestamo;
                    textBox6.Text = "" + telefono;

                    foreach (XmlNode nodoN in doc.SelectNodes("ListaPrestamo/ListaPrestamo[@nombre='" + nombre + "']/Abonos/Lista"))
                    {
                        String descripcion = nodoN.SelectSingleNode("Descripcion").InnerText;
                        String monto = nodoN.SelectSingleNode("Monto").InnerText;
                        String hora = nodoN.SelectSingleNode("Hora").InnerText;

                        dataGridView3.Rows.Add(descripcion, hora, monto);
                    }
                }
                catch (Exception)
                {
                }

            }
            
            //label106.Text = "¢" + String.Format("{0:n}", Double.Parse(monto));
            //textBox19.Text = "" + efectivo;
            //label96.Text = "¢" + String.Format("{0:n}", Double.Parse(cambio));


        }

        private void button9_Click(object sender, EventArgs e)
        {

        }

        private void button7_Click(object sender, EventArgs e)
        {
            Bloquear();
        }

        double meroMonto = 0;

        private void button11_Click(object sender, EventArgs e)
        {
            meroMonto = Double.Parse(textBox4.Text);
            label16.Text = "¢" + String.Format("{0:n}", meroMonto);
        }

        String dirListaPrestamo = Directory.GetCurrentDirectory().ToString() + "\\ListaPrestamo.xml";

        void GuardarCliente(String nombreAtt)
        {

            doc.Load(dirListaPrestamo);
            string strNamespace = doc.DocumentElement.NamespaceURI;
            XmlNode ListaPrestamo = doc.CreateNode(XmlNodeType.Element, "ListaPrestamo", strNamespace);
            XmlAttribute attr2 = doc.CreateAttribute("nombre");

            attr2.Value = nombreAtt;

            XmlNode NombrePers = doc.CreateNode(XmlNodeType.Element, "NombrePersona", null);
            XmlNode Prestamo = doc.CreateNode(XmlNodeType.Element, "Prestamo", null);
            XmlNode Telefono = doc.CreateNode(XmlNodeType.Element, "Telefono", null);
            XmlNode Abonos = doc.CreateNode(XmlNodeType.Element, "Abonos", null);


            XmlNode[] Lista = new XmlNode[dataGridView3.Rows.Count - 1];

            //Variables
            XmlNode Monto, Descripcion, Fecha;

            NombrePers.InnerText = "" + textBox3.Text;
            Prestamo.InnerText = "" + textBox4.Text;
            Telefono.InnerText = "" + textBox6.Text;

            for (int i = 0; i < dataGridView3.Rows.Count - 1; i++)
            {
                Lista[i] = doc.CreateNode(XmlNodeType.Element, "Lista", null);
                Monto = doc.CreateNode(XmlNodeType.Element, "Monto", strNamespace);
                Descripcion = doc.CreateNode(XmlNodeType.Element, "Descripcion", strNamespace);
                Fecha = doc.CreateNode(XmlNodeType.Element, "Hora", strNamespace);

                Descripcion.InnerText = "" + dataGridView3.Rows[i].Cells[0].Value;
                Fecha.InnerText = "" + dataGridView3.Rows[i].Cells[1].Value;
                Monto.InnerText = "" + dataGridView3.Rows[i].Cells[2].Value;

                
                Lista[i].AppendChild(Descripcion);
                Lista[i].AppendChild(Monto);
                Lista[i].AppendChild(Fecha);

                Abonos.AppendChild(Lista[i]);

            }


            ListaPrestamo.Attributes.Append(attr2);
            ListaPrestamo.AppendChild(NombrePers);
            ListaPrestamo.AppendChild(Prestamo);
            ListaPrestamo.AppendChild(Telefono);
            ListaPrestamo.AppendChild(Abonos);

            doc.DocumentElement.AppendChild(ListaPrestamo);
            doc.Save(dirListaPrestamo);

            dataGridView3.Rows.Clear();

        }

        void EliminarRegistro(String nombreAtt, String dir, String descendants)
        {
            
            var xDoc = XDocument.Load(dir);

            foreach (var elem in xDoc.Document.Descendants(descendants))
            {
                foreach (var attr in elem.Attributes("nombre"))
                {
                    if (attr.Value.Equals(nombreAtt))
                        elem.RemoveAll();
                }
            }

            xDoc.Save(dir);
            //MessageBox.Show("Deleted Successfully");
            
        }

        private void button12_Click(object sender, EventArgs e)
        {
            dataGridView3.Rows.Add(richTextBox1.Text,label1.Text + " - " + label2.Text, textBox5.Text);
        }

        private void dataGridView3_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            
        }

        private void dataGridView3_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
        {
            //string sobrescribir = "";
            if (e.RowIndex >= 0)
            {
                try
                {
                    val = 0;

                    for (int i = 0; i < dataGridView3.RowCount - 1; i++)
                    {

                        //Condicion para ingresos
                        if (dataGridView3.Rows[i].Cells["MontosPrestamo"].Value.ToString().Equals(""))
                        {
                            dataGridView3.Rows[i].Cells["MontosPrestamo"].Value = 0;
                        }
                        else
                        {
                            val += Double.Parse(dataGridView3.Rows[i].Cells["MontosPrestamo"].Value.ToString());
                        }



                    }

                    //label4.Text = "Total de Entradas: ¢0.00";

                    //sobrescribir = dataGridView1.Rows[e.RowIndex].Cells["Descripción"].Value.ToString();
                    //sobrescribir = sobrescribir + " -> Hora: " + label2.Text;
                    //dataGridView1.Rows[e.RowIndex].Cells["Descripción"].Value = sobrescribir;

                    label12.Text = "¢" + String.Format("{0:n}", val);

                    label16.Text = "¢" + String.Format("{0:n}", meroMonto - val);

                }
                catch (Exception ex)
                {

                }

            }
        }

        private void button13_Click(object sender, EventArgs e)
        {
            Recarga();
        }

        void Recarga()
        {
            LeerDatosPrestamo(textBox3.Text);
            meroMonto = Double.Parse(textBox4.Text);
            LeerDatosPrestamo(textBox3.Text);
        }

        private void button9_Click_1(object sender, EventArgs e)
        {
            LimpiarCampos();
        }

        private void button14_Click(object sender, EventArgs e)
        {
            CapturaPantalla("Prestamo de "+textBox3.Text);
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count > 0)
            {
                textBox3.Text = listView1.SelectedItems[0].Text;
                Recarga();
            }   
        }

        private void textBox2_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                try
                {
                    Verificar_Bloqueo();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("" + ex.Message);
                }
            }
        }

        

        private void button15_Click(object sender, EventArgs e)
        {
            groupBox9.Visible = false;
            dataGridView3.Width = 675;
        }

        private void button16_Click(object sender, EventArgs e)
        {
            groupBox9.Visible = true;
            dataGridView3.Width = 391;
            groupBox9.Width = 281;
            groupBox9.Location = new Point(418,21);
            this.Width = 745;
            this.Height = 727;
        }

        private void dataGridView3_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            Aviso(sender, e, dataGridView3);
        }

        private void Form2_VisibleChanged(object sender, EventArgs e)
        {
            string text = InsertData.TextData.ToString();
            string text2 = InsertDinero.TextData.ToString();
            string confirmado = confirmadoClick.TextData.ToString();

            text2 = text2.Replace("¢", "");
            text2 = text2.Replace(".", "");

            if (confirmado.Equals("click"))
            {
                if (!text.Equals("") || text2.Equals(""))
                {
                    dataGridView1.Rows.Add("",text2, text);
                }
            }
            
        }

        int celda, columna = 0;
        String seleccion = "";

        private void dataGridView1_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            try
            {
                if (e.Button == MouseButtons.Right)
                {
                    ContextMenu m = new ContextMenu();

                    MenuItem copiar = new MenuItem("Copiar");
                    MenuItem renombrar = new MenuItem("Renombrar");
                    MenuItem eliminar = new MenuItem("Eliminar");
                    MenuItem pegar = new MenuItem("Pegar");
                    MenuItem regresar_monto = new MenuItem("Regresar Monto");

                    m.MenuItems.Add(copiar);
                    m.MenuItems.Add(renombrar);
                    m.MenuItems.Add(eliminar);
                    m.MenuItems.Add(pegar);
                    m.MenuItems.Add(regresar_monto);

                    int currentMouseOverRow = dataGridView1.HitTest(e.X, e.Y).RowIndex;

                    /*if (currentMouseOverRow >= 0)
                    {
                        m.MenuItems.Add(new MenuItem(string.Format("Do something to row {0}", currentMouseOverRow.ToString())));
                    }*/

                    m.Show(dataGridView1, new Point(e.X, e.Y));

                    seleccion = dataGridView1[e.ColumnIndex, e.RowIndex].Value.ToString();
                    celda = e.RowIndex;
                    columna = e.ColumnIndex;

                    copiarT(seleccion);



                    eliminar.Click += new System.EventHandler(this.eliminar_Click);
                    copiar.Click += new System.EventHandler(this.copiar_Click);
                    renombrar.Click += new System.EventHandler(this.renombrar_Click);
                    pegar.Click += new System.EventHandler(this.pegar_Click);
                    regresar_monto.Click += new System.EventHandler(this.regresarMonto_Click);

                }
            }
            catch (Exception)
            {
            }
        }

        private void eliminar_Click(object sender, System.EventArgs e)
        {
            try
            {
                EliminarEntrada(celda, seleccion);
            }
            catch (Exception)
            {
            }
        }

        private void pegar_Click(object sender, System.EventArgs e)
        {
            try
            {
                pegarT(Clipboard.GetText(), celda, columna);
            }
            catch (Exception)
            {

                throw;
            }
        }

        private void regresarMonto_Click(object sender, System.EventArgs e)
        {
            RegresarMonto(celda, columna);
        }

        
        void copiarT(String texto)
        {
            Clipboard.SetText(texto);
        }

        static DataGridViewImageColumn mod = new DataGridViewImageColumn();

        void pegarT(String texto, int fila, int columna)
        {
            dataGridView1.Rows[fila].Cells[columna].Value = texto;
        }

        void RegresarMonto(int fila, int columna)
        {
            try
            {
                double monto = Double.Parse(dataGridView1.Rows[celda].Cells["Ingresos"].Value.ToString());
                dataGridView1.Rows[celda].Cells[0].Value = "";
                montos_b = montos_b - monto;
                dataGridView1.Rows[celda].DefaultCellStyle = cell_style_org;
                label14.Text = "Saldo Bloqueado: ¢" + String.Format("{0:n}", montos_b);
            }
            catch (Exception)
            {
            }
        }

        private void copiar_Click(object sender, System.EventArgs e)
        {
            copiarT(seleccion);
        }

        private void renombrar_Click(object sender, System.EventArgs e)
        {
            DataGridViewCell cell = dataGridView1.Rows[celda].Cells[columna];
            dataGridView1.CurrentCell = cell;
            dataGridView1.BeginEdit(true);

        }
        
        DataGridViewCellStyle cell_style = new DataGridViewCellStyle();
        DataGridViewCellStyle cell_style_org = new DataGridViewCellStyle();

        
        double montos_b = 0;
        String value = "";
        double ult = 0;

        string ColorPresionado = "";

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            var senderGrid = (DataGridView)sender;

            if (senderGrid.Columns[e.ColumnIndex] is DataGridViewButtonColumn &&
                e.RowIndex >= 0)
            {
                

                if (ColorPresionado.Equals("1") || dataGridView1.Rows[0].DefaultCellStyle.BackColor == Color.White)
                {
                    dataGridView1.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.DarkOliveGreen;
                    dataGridView1.Rows[e.RowIndex].DefaultCellStyle.ForeColor = Color.White;
                    
                }
                else if (ColorPresionado.Equals("2"))
                {
                    dataGridView1.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.LightSeaGreen;
                    dataGridView1.Rows[e.RowIndex].DefaultCellStyle.ForeColor = Color.White;
                    checkData.ReadOnly = true;
                }
                else if (ColorPresionado.Equals("3"))
                {
                    dataGridView1.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.DarkGreen;
                    dataGridView1.Rows[e.RowIndex].DefaultCellStyle.ForeColor = Color.White;
                }
                else if (ColorPresionado.Equals("4"))
                {
                    dataGridView1.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.Teal;
                    dataGridView1.Rows[e.RowIndex].DefaultCellStyle.ForeColor = Color.White;
                }
                else if (ColorPresionado.Equals("5"))
                {
                    dataGridView1.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.DarkSlateGray;
                    dataGridView1.Rows[e.RowIndex].DefaultCellStyle.ForeColor = Color.White;
                }
                else if (ColorPresionado.Equals("6"))
                {
                    dataGridView1.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.SeaGreen;
                    dataGridView1.Rows[e.RowIndex].DefaultCellStyle.ForeColor = Color.White;
                }

                /* 
                 
                 */

                dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = 0;
                
                try
                {
                    if (dataGridView1.Rows[e.RowIndex].Cells[0].Value.ToString().Equals("0"))
                    {
                        value = dataGridView1.Rows[e.RowIndex].Cells["Ingresos"].Value.ToString();
                        montos_b += Double.Parse(value);
                        ult = montos_b;

                        label14.Text = "Saldo Bloqueado: ¢" + String.Format("{0:n}", montos_b);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message+"\n"+ex.StackTrace);
                }
            }
        }

        List<double> MontosBloqueados = new List<double>();

        public void RowsColor(int fila)
        {
            for (int i = 0; i < dataGridView1.Rows.Count; i++)
            {
                if (dataGridView1.Rows[fila].Cells[0].Value.ToString().Equals("DarkOliveGreen"))
                    {
                        dataGridView1.Rows[fila].DefaultCellStyle.BackColor = Color.DarkOliveGreen;
                    }
                    else if (dataGridView1.Rows[fila].Cells[0].Value.ToString().Equals("Green"))
                    {
                        dataGridView1.Rows[fila].DefaultCellStyle.BackColor = Color.Green;
                }
                
            }
        }

        private void dataGridView1_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            
        }

        private void button17_Click(object sender, EventArgs e)
        {
            montos_b = 0;
            enCaja = 0;
            label14.Text = "Saldo Bloqueado: ¢" + String.Format("{0:n}", montos_b);
        }

        double enCaja = 0;

        private void textBox7_TextChanged(object sender, EventArgs e)
        {
            try
            {
                double montoN = Double.Parse(textBox7.Text.ToString());
                
                double restante = montos_b - montoN;

                enCaja = (val - val2) - montos_b + restante;

                label22.Text = "Sobrante: ¢" + String.Format("{0:n}", restante);
                label20.Text = "Saldo en Caja: ¢" + String.Format("{0:n}", enCaja);
            }
            catch (Exception)
            {
            }
        }

        private void dataGridView1_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            if (e.RowIndex < 0)
                return;

            if (e.ColumnIndex == 0)
            {
                e.Paint(e.CellBounds, DataGridViewPaintParts.All);

                var w = Properties.Resources.lock_24px.Width;
                var h = Properties.Resources.lock_24px.Height;

                var x = e.CellBounds.Left + (e.CellBounds.Width - w) / 2;
                var y = e.CellBounds.Top + (e.CellBounds.Height - h) / 2;

                e.Graphics.DrawImage(Properties.Resources.lock_24px, new Rectangle(x, y, w, h));
                e.Handled = true;

            }
        }

        private void button18_Click(object sender, EventArgs e)
        {
            ColorPresionado = "1";
            
        }

        private void button19_Click(object sender, EventArgs e)
        {
            ColorPresionado = "2";
        }

        private void button20_Click(object sender, EventArgs e)
        {
            ColorPresionado = "3";
        }

        private void button21_Click(object sender, EventArgs e)
        {
            ColorPresionado = "4";
        }

        private void button22_Click(object sender, EventArgs e)
        {
            ColorPresionado = "5";
        }

        private void button23_Click(object sender, EventArgs e)
        {
            ColorPresionado = "6";
        }

        private void button24_Click(object sender, EventArgs e)
        {
            try
            {
                string captura = textBox7.Text.ToString();
                richTextBox2.AppendText("Se retiro un monto de ¢" + String.Format("{0:n}", captura) + " \n " + label22.Text+" \n ");
            }
            catch (Exception)
            {
            }
        }

        private void label21_Click(object sender, EventArgs e)
        {

        }

        void EliminarBitacora(int indice)
        {
            //listView2.Items.RemoveAt(indice);
        }

        private void listView2_DoubleClick(object sender, EventArgs e)
        {
            /*int indice = 0;

            if (listView2.SelectedItems.Count > 0)
            {
                indice = listView2.Items.IndexOf(listView2.SelectedItems[0]);
            }
            EliminarBitacora(indice);*/
        }

        void EliminarEntrada(int fila, string valor)
        {
            dataGridView1.Rows.Remove(dataGridView1.Rows[fila]);
        }

        void MostrarDate()
        {
            LeerDatosIngresos(monthCalendar1.SelectionRange.Start.ToShortDateString());
            LeerDatosEgresos(monthCalendar1.SelectionRange.Start.ToShortDateString());
            LeerDatosBitacora(monthCalendar1.SelectionRange.Start.ToShortDateString());

            SumarIngresos();
            SumarEgresos();
        }

        void Eliminar(String nombreAtt, String dir, String descendants)
        {
            //doc.Load(dir);


            //XmlNode t = doc.SelectSingleNode("ListaIngreso/ListaIngreso[@fecha='" + nombreAtt + "']");
            //t.RemoveChild(t);
            //doc.Save(dirListaMonto);
            //MessageBox.Show("Test");


            var xDoc = XDocument.Load(dir);

            foreach (var elem in xDoc.Document.Descendants(descendants))
            {
                foreach (var attr in elem.Attributes("fecha"))
                {
                    if (attr.Value.Equals(nombreAtt))
                        elem.RemoveAll();
                }
            }

            xDoc.Save(dir);
            //MessageBox.Show("Deleted Successfully");


        }
    }
}
