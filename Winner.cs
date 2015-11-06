using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlClient;
using System.Configuration;
using System.Data;

public partial class Admin_Winners : System.Web.UI.Page
{
    string cmd;
    int result;
    string command;
    SqlDataReader reader;
    Database db = new Database();
    DataTable dt;
    DatabaseExecution AS = new DatabaseExecution();
    string School_Name;
    string Connection_String = ConfigurationManager.ConnectionStrings["MADSKOOL"].ConnectionString;
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            BindEventDetatils();
            LoadEventDate();
        }
    }

    protected void BindEventDetatils()
    {
        dt = new DataTable();
        using (SqlConnection con = new SqlConnection(Connection_String))
        {
            con.Open();
            SqlCommand cmd = new SqlCommand("Select * from MSCK_WiinersList_Details", con);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(dt);
            con.Close();
        }
        grdNewEvent.DataSource = dt;
        grdNewEvent.DataBind();
    }




  


  




 





    protected void GridView1_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        int index = Convert.ToInt32(e.CommandArgument);
        if (e.CommandName.Equals("detail"))
        {
            string Student_Name = grdNewEvent.DataKeys[index].Value.ToString();
            IEnumerable<DataRow> query = from i in dt.AsEnumerable()
                                         where i.Field<String>("Student_Name").Equals(Student_Name)
                                         select i;
            DataTable detailTable = query.CopyToDataTable<DataRow>();
            DetailsView1.DataSource = detailTable;
            DetailsView1.DataBind();
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.Append(@"<script type='text/javascript'>");
            sb.Append("$('#detailModal').modal('show');");
            sb.Append(@"</script>");
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "DetailModalScript", sb.ToString(), false);
        }
        else if (e.CommandName.Equals("editRecord"))
        {
            GridViewRow gvrow = grdNewEvent.Rows[index];
            lblEditWiinerID.Text = HttpUtility.HtmlDecode(gvrow.Cells[3].Text).ToString();
            drpEditStudentName.SelectedItem.Text = HttpUtility.HtmlDecode(gvrow.Cells[4].Text).ToString();
            drpEditCategoryLevel.SelectedItem.Text = HttpUtility.HtmlDecode(gvrow.Cells[5].Text);
            txtEditStudentStandard.Text = HttpUtility.HtmlDecode(gvrow.Cells[6].Text);
            txtEditStudentSchool.Text = HttpUtility.HtmlDecode(gvrow.Cells[7].Text);
            txtEditPrize.Text = HttpUtility.HtmlDecode(gvrow.Cells[8].Text);
            lblResult.Visible = false;
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.Append(@"<script type='text/javascript'>");
            sb.Append("$('#editModal').modal('show');");
            sb.Append(@"</script>");
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "EditModalScript", sb.ToString(), false);

        }
        else if (e.CommandName.Equals("deleteRecord"))
        {
            string code = grdNewEvent.DataKeys[index].Value.ToString();
            hfCode.Value = code;
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.Append(@"<script type='text/javascript'>");
            sb.Append("$('#deleteModal').modal('show');");
            sb.Append(@"</script>");
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "DeleteModalScript", sb.ToString(), false);
        }

    }



    // Handles Update Button Click Event
    protected void btnSave_Click(object sender, EventArgs e)
    {
        string WinnerID = lblEditWiinerID.Text;
        string EvenDate = drpEditEventDate.SelectedItem.Text;
        string EventName = drpEditEventName.SelectedItem.Text;
        string StudentName =  drpEditStudentName.SelectedItem.Text ;
        string StudentCategory = drpEditCategoryLevel.SelectedItem.Text;
        string StudentStandard = txtEditStudentStandard.Text ;
        string StudentSchool = txtEditStudentSchool.Text;
        string StudentPrize = txtEditPrize.Text;

        UpdatingData(EvenDate, EventName, StudentName, StudentCategory, StudentStandard, StudentSchool, StudentPrize, WinnerID);

        BindEventDetatils();
        System.Text.StringBuilder sb = new System.Text.StringBuilder();
        sb.Append(@"<script type='text/javascript'>");
        sb.Append("alert('Records Updated Successfully');");
        sb.Append("$('#editModal').modal('hide');");
        sb.Append(@"</script>");
        ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "EditHideModalScript", sb.ToString(), false);
    }

    #region[Updating Data]
    private void UpdatingData(string PEvenDate, string PEventName, string PStudentName, string PStudentCategory, string PStudentStandard, string PStudentSchool, string PStudentPrize, string PWinnerID)
    {
        cmd = "update MSCK_WiinersList_Details set Event_Date='" + PEvenDate.ToString() + "',Event_Name='" + PEventName.ToString() + "',Category='" + PStudentCategory.ToString() + "',Student_Name='" + PStudentName.ToString() + "',School_Name='" + PStudentSchool.ToString() + "',Prize='" + PStudentPrize.ToString() + "'  where Winners_ID='" + PWinnerID.ToString() + "'   ";


        result = AS.DML(cmd);
        if (result > 0)
        {
            int a2 = 1;
            //            lblErrorMsg.ForeColor = System.Drawing.Color.Green;
            //          lblErrorMsg.Text = "Blood Ordered Successfull";
            //        clear();
        }

        else
        {
            int b2 = 1;
            // lblErrorMsg.ForeColor = System.Drawing.Color.Red;
            //lblErrorMsg.Text = "Process Failed";

        }
    }

    #endregion
  


    protected void drpAddEventDate_SelectedIndexChanged(object sender, EventArgs e)
    {
        LoadEventName();
    }



    protected void drpADDCategoryLevel_SelectedIndexChanged(object sender, EventArgs e)
    {
        LoadStudentName();
    } 





    //Handles Add record button click in main form
    protected void btnAdd_Click(object sender, EventArgs e)
    {
        System.Text.StringBuilder sb = new System.Text.StringBuilder();
        sb.Append(@"<script type='text/javascript'>");
        sb.Append("$('#addModal').modal('show');");
        sb.Append(@"</script>");
        ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "AddShowModalScript", sb.ToString(), false);

    }

    //Handles Add button click in add modal popup
    protected void btnAddRecord_Click(object sender, EventArgs e)
    {
        string EventName = drpAddEventName.SelectedItem.Text;
        string EvenDate = drpAddEventDate.Text;
        string Category = drpADDCategoryLevel.SelectedItem.Text;
        string Strudent_Name = drpAddStudentName.SelectedItem.Text;
        string School_Name = txtAddStudentSchool.Text;
        string Prize = txtAddPrizeReward.Text;
        insertingData(EventName, EvenDate, Category, Strudent_Name, School_Name, Prize);
        BindEventDetatils();
        System.Text.StringBuilder sb = new System.Text.StringBuilder();
        sb.Append(@"<script type='text/javascript'>");
        sb.Append("alert('Record Added Successfully');");
        sb.Append("$('#addModal').modal('hide');");
        sb.Append(@"</script>");
        ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "AddHideModalScript", sb.ToString(), false);
    }


    #region[Inserting Data]
    private void insertingData(string PEventName, string PEventDate, string PCategory, string PStrudent_Name, string PSchool_Name, string PPrize)
    {

        cmd = "insert into MSCK_WiinersList_Details(Event_Date,Event_Name,Category,Student_Name,School_Name,Prize) values('" + drpAddEventDate.SelectedItem.Text + "','" + drpAddEventName.SelectedItem.Text + "','" + drpADDCategoryLevel.SelectedItem.Text + "','" + txtAddStudentSchool.Text + "','" + txtAddStudentSchool.Text + "','" + txtAddPrizeReward.Text + "')";

        result = AS.DML(cmd);
        if (result > 0)
        {
            int a1 = 1;
            //  lblErrorMsg.ForeColor = System.Drawing.Color.Green;
            //lblErrorMsg.Text = "Blood Ordered Successfull";
            //clear();
        }

        else
        {
            int b1 = 1;
            //  lblErrorMsg.ForeColor = System.Drawing.Color.Red;
            // lblErrorMsg.Text = "Process Failed";

        }
    }

    #endregion



   // Handles Delete button click in delete modal popup
    protected void btnDelete_Click(object sender, EventArgs e)
    {
        string DeleteEventName = hfCode.Value;
        DeletingData(DeleteEventName);
        BindEventDetatils();
        System.Text.StringBuilder sb = new System.Text.StringBuilder();
        sb.Append(@"<script type='text/javascript'>");
        sb.Append("alert('Record deleted Successfully');");
        sb.Append("$('#deleteModal').modal('hide');");
        sb.Append(@"</script>");
        ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "delHideModalScript", sb.ToString(), false);
    }



    #region[Deleting Data]
    private void DeletingData(string code)
    {
        cmd = "delete from MSCK_WiinersList_Details where Event_Name='" + code.ToString() + "'";

        //cmd = "insert into Event_Details(Event_Name,Event_Date,Event_Venue,Brief_Description,Country,Date,Units_Needed,Purpose) values('" + txtUsername.Text + "','" + drpBloodneeded.SelectedItem.Text + "','" + State.ToString() + "','" + City.ToString() + "','" + LblshowCountry.Text + "','" + txtdate.Text + "','" + drpunitsneeded.SelectedItem.Text + "','" + txtpurpose.Text + "')";

        result = AS.DML(cmd);
        if (result > 0)
        {
            int a3 = 1;
            // lblErrorMsg.ForeColor = System.Drawing.Color.Green;
            //lblErrorMsg.Text = "Blood Ordered Successfull";
            //clear();
        }

        else
        {
            int b3 = 1;
            //    lblErrorMsg.ForeColor = System.Drawing.Color.Red;
            //  lblErrorMsg.Text = "Process Failed";

        }
    }

    #endregion




    #region[Loading StudentName]
    public void LoadStudentName()
    {
       
        DataTable subjects = new DataTable();

        using (SqlConnection con = new SqlConnection(Connection_String))
        {

            try
            {
                SqlDataAdapter adapter = new SqlDataAdapter("SELECT distinct Student_Name,School_Name where Student_Category='"+ drpADDCategoryLevel.SelectedItem.Text +"'", con);
                adapter.Fill(subjects);
                foreach (DataRow row in subjects.Rows)
                {
                    School_Name = row.Field<string>("School_Name");
                }
                txtAddStudentSchool.Text = School_Name.ToString();

                drpAddStudentName.DataSource = subjects;
                drpAddStudentName.DataTextField = "Student_Name";
                drpAddStudentName.DataValueField = "Student_ID";
                drpAddStudentName.DataBind();
            }
            catch (Exception ex)
            {
                // Handle the error
            }

        }

        // Add the initial item - you can add this even if the options from the
        // db were not successfully loaded
        drpAddEventName.Items.Insert(0, new ListItem("<Select Event Name>", "0"));

    }
    #endregion



    
    #region[Loading EvenName]
    public void LoadEventName()
    {

        DataTable subjects = new DataTable();

        using (SqlConnection con = new SqlConnection(Connection_String))
        {

            try
            {
                SqlDataAdapter adapter = new SqlDataAdapter("SELECT distinct Event_ID, Event_Name FROM MSCK_EventDetails where Event_Date='" + drpAddEventDate.SelectedItem.Text + "'", con);
                adapter.Fill(subjects);

                drpAddEventName.DataSource = subjects;
                drpAddEventName.DataTextField = "Event_Name";
                drpAddEventName.DataValueField = "Event_ID";
                drpAddEventName.DataBind();
            }
            catch (Exception ex)
            {
                // Handle the error
            }

        }

        // Add the initial item - you can add this even if the options from the
        // db were not successfully loaded
        drpAddEventName.Items.Insert(0, new ListItem("<Select Event Name>", "0"));

    }
    #endregion


    #region[Loading EvenDate]
    public void LoadEventDate()
    {

        DataTable subjects = new DataTable();

        using (SqlConnection con = new SqlConnection(Connection_String))
        {

            try
            {
                SqlDataAdapter adapter = new SqlDataAdapter("SELECT distinct Event_ID, Event_Date FROM MSCK_EventDetails", con);
                adapter.Fill(subjects);

                drpAddEventDate.DataSource = subjects;
                drpAddEventDate.DataTextField = "Event_Date";
                drpAddEventDate.DataValueField = "Event_ID";
                drpAddEventDate.DataBind();
            }
            catch (Exception ex)
            {
                // Handle the error
            }

        }

        // Add the initial item - you can add this even if the options from the
        // db were not successfully loaded
        drpAddEventDate.Items.Insert(0, new ListItem("<Select Event Date>", "0"));

    }
    #endregion

}
