<%@ Page Title="Home Page" Language="VB" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.vb" Inherits="ZipPdfTest._Default" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">

    <div class="jumbotron">
        <h1>Zip PDF Test</h1>
    </div>
    <div class="row">
        <div class="col-md-6">
            <label>File name:</label>
            <asp:TextBox ID="textboxFileName" runat="server"></asp:TextBox>
        </div>
        <div class="col-md-6">
            <asp:Button ID="buttonTestCompression" runat="server" Text="TestCompression" />
            <asp:Button ID="buttonTestNormal" runat="server" Text="TestNormal" />
            <asp:Button ID="buttonDanTest" runat="server" Text="DanTest" />
        </div>
        <div>
            <asp:Label ID="labelMessages" runat="server" />
        </div>:
    </div>

</asp:Content>
