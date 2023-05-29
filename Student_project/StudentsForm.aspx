<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="StudentsForm.aspx.cs" Inherits="Student_project.StudentsForm" %>

<!DOCTYPE html>
<html>
<head>
    <title>Form</title>
    <style>
        .form-container {
            max-width: 400px;
            margin: 0 auto;
            padding: 20px;
            background-color: #f5f5f5;
            border: 1px solid #ddd;
            border-radius: 5px;
        }

        .form-field {
            margin-bottom: 10px;
        }

        .form-label {
            display: block;
            font-weight: bold;
        }

        .form-input {
            width: 100%;
            padding: 8px;
            border: 1px solid #ddd;
            border-radius: 4px;
        }

        .form-submit {
            margin-top: 10px;
        }

        .form-table {
            width: 100%;
            margin-top: 20px;
            border-collapse: collapse;
        }

            .form-table th,
            .form-table td {
                padding: 8px;
                border: 1px solid #ddd;
            }

        .form-download-link {
            display: block;
            margin-top: 10px;
        }
    </style>
</head>
<body>
    <div class="form-container">
        <form runat="server" enctype="multipart/form-data">
            <div class="form-field">
                <label for="studentName" class="form-label">Name of the Student:</label>
                <asp:TextBox ID="studentName" runat="server" CssClass="form-input" required></asp:TextBox>
            </div>
            <div class="form-field">
                <label for="instituteName" class="form-label">Institute Name:</label>
                <asp:TextBox ID="instituteName" runat="server" CssClass="form-input" required></asp:TextBox>
            </div>
            <div class="form-field">
                <label for="courseName" class="form-label">Course Name:</label>
                <asp:TextBox ID="courseName" runat="server" CssClass="form-input" required></asp:TextBox>
            </div>
            <div class="form-field">
                <label for="courseStartDateTime" class="form-label">Course Start Date and Time:</label>
                <asp:TextBox ID="courseStartDateTime" runat="server" placeholder="mm/dd/yyyy hh:mm:ss" TextMode="DateTimeLocal" CssClass="form-input" required></asp:TextBox>
            </div>
            <div class="form-field">
                <label for="pdfFile" class="form-label">Upload PDF:</label>
                <asp:FileUpload ID="pdfFile" runat="server" CssClass="form-input" required></asp:FileUpload>
            </div>
            <asp:Button ID="Submit" runat="server" Text="Submit" CssClass="form-submit" OnClick="submitButton_Click" />
            <asp:GridView ID="dataGridView" runat="server" CssClass="form-table" AutoGenerateColumns="false">
                <Columns>
                    <asp:BoundField DataField="Name" HeaderText="Student Name" />
                    <asp:BoundField DataField="Institute" HeaderText="Institute Name" />
                    <asp:BoundField DataField="Course" HeaderText="Course Name" />
                    <asp:BoundField DataField="StartDateTime" HeaderText="Course Start Date and Time" />
                    <asp:TemplateField HeaderText="Download PDF">
                        <ItemTemplate>
                            <asp:LinkButton ID="Download" runat="server" Text="Download PDF" CssClass="form-download-link"
                                OnCommand="downloadLink_Command" CommandArgument='<%# Eval("StudentId") %>'></asp:LinkButton>
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
            </asp:GridView>
        </form>

    </div>
</body>
</html>
