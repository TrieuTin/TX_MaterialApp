/*
 * Hi! Coder
 * My name's Tin, when I wrote this code, only God and i knew how it worked.
 * But now, only God knows it!
 * Therefore, if you are trying to optimize this rountine and it fails (most surely), please contact me via 0824.272.373 
 * This thing is not help you, but I promise I will give you spiritual support
 * Good lucky my brothers
 */
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace MaterialTracking.ViewModels
{
  public  class AutoCuttingJobTicketTrackingViewModel
    {
        public ObservableCollection<Model_AutoCuttingJob> MyData1 { get; set; }
        public AutoCuttingJobTicketTrackingViewModel(string _machine)
        {
            GetData(_machine);
        }
        public void GetData(string _ma)
        {
            var dt = Class.ServerData.Data.AutoCuttingView(_ma);
            MyData1 = new ObservableCollection<Model_AutoCuttingJob>();
            //MyData1.Add(new Model_AutoCuttingJob
            //{
            //    Machine = "dfasdfasdfasdf"
            //});
            string tem_pronp = string.Empty;
            string tem_ry= string.Empty;
            string tem_Art= string.Empty;
            string tem_Model= string.Empty;
            string tem_MatID= string.Empty;
            string tem_Line= string.Empty; 
            
            string _pronp = string.Empty;
            string _ry= string.Empty;
            string _Art= string.Empty;
            string _Model= string.Empty;
            string _MatID= string.Empty;
            string _Line= string.Empty;
            
            
            
            foreach (System.Data.DataRow r in dt.Rows)
            {
                try
                {
                    //var temp = new Model_AutoCuttingJob();
                    //temp.ProNo = r["Prono"].ToString();
                    //temp.Machine = r["Machine"].ToString();
                    //temp.Ry = r["Ry"].ToString();
                    //temp.Art = r["Article"].ToString();
                    //temp.ModelName = r["ModelName"].ToString();
                    //temp.MatID = r["CLBH"].ToString();
                    //temp.Line = r["Lean"].ToString();
                    //temp.Component = r["Component"].ToString();
                    //temp.TargetQty = r["TargetQty"].ToString();
                    //temp.TotalProcessedQty = r["TotalProcessQty"].ToString();
                    //temp.SuperMarketET = r["Supermaket"].ToString();
                    //temp.ProductionDate = ((DateTime)r["ProductionDate"]).ToString("yyyy-MM-dd");
                    //MyData.Add(temp);
                    if (r["prono"].ToString() == "")
                    {
                        _pronp = tem_pronp = ".";
                    }
                    else
                    {

                        if (_pronp != r["Prono"].ToString())
                        {
                            _pronp = r["prono"].ToString();
                            tem_pronp = _pronp;
                        }
                        else tem_pronp = "";
                    }
                    
                    if (r["Ry"].ToString() == "")
                    {
                        tem_ry = _ry = ".";
                    }
                    else
                    {
                        if (_ry != r["ry"].ToString())
                        {
                            _ry = r["ry"].ToString();
                            tem_ry = _ry;
                        }
                        else tem_ry = "";
                    }

                    if (r["Article"].ToString() == "")
                    {
                        tem_Art = _Art = ".";
                    }
                    else
                    {

                        if (_Art != r["article"].ToString())
                        {
                            _Art = r["article"].ToString();
                            tem_Art = _Art;
                        }
                        else tem_Art = "";
                    }

                    if (r["ModelName"].ToString() == "")
                    {
                        _Model = tem_Model = ".";
                    }
                    else
                    {
                        if (_Model != r["ModelName"].ToString())
                        {
                            _Model = r["ModelName"].ToString(); ;
                            tem_Model = _Model;
                        }
                        else tem_Model= "";
                    }

                    if (r["CLBH"].ToString() == "")
                    {
                        tem_MatID = _MatID = ".";
                        
                    }
                    else
                    { 
                        if (_MatID != r["CLBH"].ToString()) 
                        {
                            _MatID = r["CLBh"].ToString(); ;
                            tem_MatID = _MatID;
                        }
                        else tem_MatID= "";
                    }

                    if (r["Lean"].ToString() =="")
                    {
                        tem_Line = _Line = ".";
                       
                    }
                    else
                    {
                        if (_Line != r["Lean"].ToString())
                        {
                            _Line = r["Lean"].ToString(); ;
                            tem_Line = _Line;
                        }
                        else tem_Line = "";
                    }
                   

                    MyData1.Add(new Model_AutoCuttingJob
                    {
                        ProNo = _pronp,
                        Machine = r["Machine"].ToString(),
                        Ry =_ry,
                        Art = _Art,
                        ModelName = _Model,
                        MatID = _MatID,
                        Line = _Line,
                        Component = r["Component"].ToString(),
                        TargetQty = r["TargetQty"].ToString(),
                        TotalProcessedQty = r["TotalProcessQty"].ToString(),
                        SuperMarketET = r["Supermaket"].ToString(),
                        ProductionDate = ((DateTime)r["ProductionDate"]).ToString("yyyy-MM-dd")
                        , ColorProno = tem_pronp == "" ? System.Drawing.Color.White:System.Drawing.Color.Black,
                        ColorArt =tem_Art ==""? System.Drawing.Color.White:System.Drawing.Color.Black,
                        ColorLine=tem_Line ==""? System.Drawing.Color.White:System.Drawing.Color.Black,
                        ColorMat=tem_MatID ==""? System.Drawing.Color.White:System.Drawing.Color.Black,
                        ColorModelName=tem_Model ==""? System.Drawing.Color.White:System.Drawing.Color.Black,
                        ColorRy=tem_ry ==""? System.Drawing.Color.White:System.Drawing.Color.Black,
                    });
                }
                catch (Exception xx)
                {

                    throw;
                }

            }
        }
    }
   public class Model_AutoCuttingJob
    {
        public string Machine { get; set; }
        public string ProNo { get; set; }
        public string Ry { get; set; }
        public string Art { get; set; }
        public string ModelName { get; set; }
      
        public string MatID { get; set; }
        public string Line { get; set; }
        public string Component { get; set; }
        public string TargetQty { get; set; }
        public string TotalProcessedQty { get; set; }
        public string SuperMarketET { get; set; }
        public string ProductionDate { get; set; }
        public System.Drawing.Color ColorRy { get; set; }
        public System.Drawing.Color ColorProno { get; set; }
        public System.Drawing.Color ColorArt { get; set; }
        public System.Drawing.Color ColorModelName { get; set; }
        public System.Drawing.Color ColorMat { get; set; }
        public System.Drawing.Color ColorLine { get; set; }
    }
}

