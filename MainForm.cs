using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using HSIAPIClient.Models;
using Newtonsoft.Json;

namespace HSIAPIClient
{
    public partial class MainForm : Form
    {
        private string _accessToken = "";

        private List<ValueText> planList = new List<ValueText>()
        {
            new ValueText() { Text = "My Endowment", Value = "10" },
            new ValueText() { Text = "My Retirement", Value = "11" },
            new ValueText() { Text = "My Off Springs", Value = "12" },
            new ValueText() { Text = "My Junior Life", Value = "20" },
            new ValueText() { Text = "My Life Security", Value = "30" },
            new ValueText() { Text = "My Life Cover", Value = "40" },
            new ValueText() { Text = "My Life Benefits", Value = "50" },
            new ValueText() { Text = "My Funeral", Value = "90" }
        };

        private List<ValueText> payModeList = new List<ValueText>()
        {
            new ValueText() { Text = "Monthly", Value = "0" }, new ValueText() { Text = "Quarterly", Value = "1" },
            new ValueText() { Text = "Half Yearly", Value = "2" }, new ValueText() { Text = "Yearly", Value = "3" },
        };

        private List<ValueText> genderList = new List<ValueText>()
        {
            new ValueText() { Text = "Male", Value = "M" }, new ValueText() { Text = "Female", Value = "F" }
        };

        private List<ValueText> categoryList = new List<ValueText>()
        {
            new ValueText() { Text = "Principle Holder", Value = "A" },
            new ValueText() { Text = "Spouse", Value = "B" },
            new ValueText() { Text = "Children", Value = "C" },
            new ValueText() { Text = "Parents / Parents in Law", Value = "D" },
            new ValueText() { Text = "Adult Dependent", Value = "E" }
        };

        public MainForm()
        {
            InitializeComponent();
            cmbPlan.DataSource = planList;
            cmbPlan.DisplayMember = "Text";
            cmbPlan.ValueMember = "Value";
            cmbPayMode.DataSource = payModeList;
            cmbPayMode.DisplayMember = "Text";
            cmbPayMode.ValueMember = "Value";
            cmbGender.DataSource = genderList;
            cmbGender.DisplayMember = "Text";
            cmbGender.ValueMember = "Value";
            cmbCategory.DataSource = categoryList;
            cmbCategory.DisplayMember = "Text";
            cmbCategory.ValueMember = "Value";
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            txtClient.Text = "";
            txtSecretKey.Text = "";
            txtSumAssured.Text = "100000";
            txtanb.Text = "31";
            txtTerm.Text = "10";
        }

        private void btnAuthenticate_Click(object sender, EventArgs e)
        {
            string authenticateUrl = "http://197.248.42.157:97/hsavy_api/public/authenticate";
            var client = new HttpClient();

            AuthenticateData authenticateData = new AuthenticateData();
            authenticateData.client = txtClient.Text;
            authenticateData.secret_key = txtSecretKey.Text;
            var json = JsonConvert.SerializeObject(authenticateData);
            var data = new StringContent(json, Encoding.UTF8, "application/json");
            var response = client.PostAsync(authenticateUrl, data);
            if (response.Result.StatusCode == HttpStatusCode.OK)
            {
                var responseContent = response.Result.Content.ReadAsStringAsync();
                if (responseContent.Result.Contains(
                        "The user credentials were incorrect. Make sure your access token is valid!!"))
                {
                    lblStatusMessage.Text = "Authentication Failed";
                }
                else
                {
                    var responseJson =
                        Newtonsoft.Json.JsonConvert.DeserializeObject<AuthenticateResponse>(responseContent.Result);
                    _accessToken = responseJson?.access_token;
                    if (!string.IsNullOrEmpty(_accessToken))
                    {
                        lblStatusMessage.Text = "Succesfully Authenticated";
                    }
                }
            }
            else
            {
                lblStatusMessage.Text = "Unexpected Error";
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var plan = (string)cmbPlan.SelectedValue;
            var payMode = (string)cmbPayMode.SelectedValue;
            var gender = (string)cmbGender.SelectedValue;
            var category = (string)cmbCategory.SelectedValue;
            var sumAssured = txtSumAssured.Text;
            var anb = txtanb.Text;
            var term = txtTerm.Text;
            CalculateRetirement(plan, payMode, gender, category, sumAssured, anb, term);
        }

        private void CalculateRetirement(string plan, string payMode, string gender, string category, string sumAssured,
            string anb, string term)
        {
            string retirementUrl = GetUrl(plan);
            var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", " " + _accessToken);
            RetirementData retirementData = new RetirementData();
            retirementData.plan_code = plan;
            retirementData.anb = anb;
            retirementData.paymode = payMode;
            retirementData.gender = gender;
            retirementData.sum_assured = sumAssured;
            retirementData.term = term;
            var json = JsonConvert.SerializeObject(retirementData);
            var data = new StringContent(json, Encoding.UTF8, "application/json");
            var response = client.PostAsync(retirementUrl, data);
            if (response.Result.StatusCode == HttpStatusCode.OK)
            {
                var responseContent = response.Result.Content.ReadAsStringAsync();
                var planResponseJson =
                    Newtonsoft.Json.JsonConvert.DeserializeObject<PlanResponseData>(responseContent.Result);
                var _model_prem = planResponseJson?.modal_prem;
                var _model_status = planResponseJson?.success;
                var _model_message = planResponseJson?.message;

                if (!string.IsNullOrEmpty(_accessToken))
                {
                    lblModelPremium.Text = _model_prem;
                    lblMessage.Text = _model_message;
                    lblStatus.Text = _model_status.ToString();
                }
            }
        }

        private string GetUrl(string plan)
        {
            string url = "";
            switch (plan)
            {
                case "10":
                    url = "http://197.248.42.157:97/hsavy_api/public/api/premiums/myEndowment";
                    break;
                case "11":
                    url = "http://197.248.42.157:97/hsavy_api/public/api/premiums/myRetirement";
                    break;
                case "12":
                    url = "http://197.248.42.157:97/hsavy_api/public/api/premiums/myOffsprings";
                    break;
                case "20":
                    url = "http://197.248.42.157:97/hsavy_api/public/api/premiums/JuniorLife";
                    break;
                case "30":
                    url = "http://197.248.42.157:97/hsavy_api/public/api/premiums/myLifeSecurity";
                    break;
                case "40":
                    url = "http://197.248.42.157:97/hsavy_api/public/api/premiums/myLifeCover";
                    break;
                case "50":
                    url = "http://197.248.42.157:97/hsavy_api/public/api/premiums/myLifeBenefits";
                    break;
                case "90":
                    url = "http://197.248.42.157:97/hsavy_api/public/api/premiums/myFuneral";
                    break;
                default:
                    throw new Exception("Not Valid Url");
            }
            return url;
        }
    }
}