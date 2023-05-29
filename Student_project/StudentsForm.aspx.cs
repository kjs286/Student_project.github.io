using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using ZXing;
using ZXing.Common;
using Image = iTextSharp.text.Image;

namespace Student_project
{
    public partial class StudentsForm : System.Web.UI.Page
    {
        string connectionString = ConfigurationManager.ConnectionStrings["MyConn"].ConnectionString;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // Bind the GridView with data on page load
                BindGridView();
            }
        }

        protected void submitButton_Click(object sender, EventArgs e)
        {
                // Save the uploaded PDF file to the server
                string fileName = Path.GetFileName(pdfFile.FileName);
                string filePath = Server.MapPath("~/Uploads/" + fileName);
                pdfFile.SaveAs(filePath);

                // Insert the form data into the database
                InsertFormData(studentName.Text, instituteName.Text, courseName.Text, Convert.ToDateTime(courseStartDateTime.Text), fileName);

                // Bind the GridView with updated data
                BindGridView();

                // Clear the form fields
                ClearForm();
        }

        protected void downloadLink_Command(object sender, CommandEventArgs e)
        {
            // Retrieve the PDF file name based on the command argument (ID)
            int studentId = Convert.ToInt32(e.CommandArgument);
            string fileName = GetFileNameFromId(studentId);

            if (!string.IsNullOrEmpty(fileName))
            {
                // Download the PDF file
                // Generate QR code using student ID
                string filePath = Server.MapPath("~/Uploads/" + fileName);
                string qrCodeContent = $"Student ID: {studentId}";

                string qrCodeImagePath = Server.MapPath("~/QRCodes/" + studentId + ".png");
                GenerateQRCode(qrCodeContent, qrCodeImagePath);

                // Modify the uploaded PDF by placing the QR code
                string modifiedPdfPath = Server.MapPath("~/ModifiedPDFs/" + fileName);
                ModifyPdfWithQRCode(filePath, qrCodeImagePath, modifiedPdfPath);

                // Download the modified PDF
                Response.Clear();
                Response.ContentType = "application/pdf";
                Response.AppendHeader("Content-Disposition", "attachment; filename=" + fileName);
                Response.TransmitFile(modifiedPdfPath);
                Response.End();
            }
        }

        protected void GenerateQRCode(string content, string imagePath)
        {
            BarcodeWriter barcodeWriter = new BarcodeWriter();
            barcodeWriter.Format = BarcodeFormat.QR_CODE;

            EncodingOptions encodingOptions = new EncodingOptions();
            encodingOptions.Height = 300;
            encodingOptions.Width = 300;

            barcodeWriter.Options = encodingOptions;
            Bitmap bitmap = barcodeWriter.Write(content);

            bitmap.Save(imagePath);
        }

        protected void ModifyPdfWithQRCode(string inputPdfPath, string qrCodeImagePath, string outputPdfPath)
        {
            using (FileStream fs = new FileStream(inputPdfPath, FileMode.Open, FileAccess.Read))
            {
                PdfReader reader = new PdfReader(fs);
                using (FileStream outStream = new FileStream(outputPdfPath, FileMode.Create, FileAccess.Write))
                {
                    PdfStamper stamper = new PdfStamper(reader, outStream);
                    PdfContentByte content = stamper.GetOverContent(1);

                    Image qrCodeImage = Image.GetInstance(qrCodeImagePath);
                    qrCodeImage.ScaleAbsolute(80, 90);
                    qrCodeImage.SetAbsolutePosition(135, 50);
                    content.AddImage(qrCodeImage);

                    stamper.Close();
                }
                reader.Close();
            }
        }

        private void BindGridView()
        {
            DataTable dt = GetFormData();

            // Bind the DataTable to the GridView
            dataGridView.DataSource = dt;
            dataGridView.DataBind();
        }

        private DataTable GetFormData()
        {
            DataTable dt = new DataTable();

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string query = "SELECT StudentId, Name, Institute, Course, StartDateTime, FileName  FROM StudentTable";
                SqlCommand cmd = new SqlCommand(query, con);
                SqlDataAdapter da = new SqlDataAdapter(cmd);

                con.Open();
                da.Fill(dt);
            }

            return dt;
        }

        private void InsertFormData(string studentName, string instituteName, string courseName, DateTime courseStartDateTime, string fileName)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string query = "INSERT INTO StudentTable (Name, Institute, Course, StartDateTime, FileName) " +
                               "VALUES (@Name, @Institute, @Course, @StartDateTime, @FileName)";
                SqlCommand cmd = new SqlCommand(query, con);

                // Add parameters to the SqlCommand
                cmd.Parameters.AddWithValue("@Name", studentName);
                cmd.Parameters.AddWithValue("@Institute", instituteName);
                cmd.Parameters.AddWithValue("@Course", courseName);
                cmd.Parameters.AddWithValue("@StartDateTime", courseStartDateTime);
                cmd.Parameters.AddWithValue("@FileName", fileName);

                con.Open();
                cmd.ExecuteNonQuery();
            }
        }

        private string GetFileNameFromId(int id)
        {
            string fileName = string.Empty;

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string query = "SELECT FileName FROM StudentTable WHERE StudentId = @Id";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@Id", id);

                con.Open();
                object result = cmd.ExecuteScalar();

                if (result != null)
                {
                    fileName = result.ToString();
                }
            }

            return fileName;
        }

        private void ClearForm()
        {
            studentName.Text = string.Empty;
            instituteName.Text = string.Empty;
            courseName.Text = string.Empty;
            courseStartDateTime.Text = string.Empty;
        }
    }
}