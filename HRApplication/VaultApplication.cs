using MFiles.VAF;
using MFiles.VAF.Common;
using MFiles.VAF.Configuration;
using MFilesAPI;
using System;
using System.Diagnostics;

namespace HRApplication
{
    /// <summary>
    /// The entry point for this Vault Application Framework application.
    /// </summary>
    /// <remarks>Examples and further information available on the developer portal: http://developer.m-files.com/. </remarks>
    public class VaultApplication
       : VaultApplicationBase
    {

        int p_Employee_Name = 1058;
        int p_Employee_Code = 1392;
        int p_Leave_Type = 1132;
        int p_Adjust_Lates_To = 1566;
        int p_Start_Date = 1133;
        int p_End_Date = 1134;
        int p_NoofLates_monthly = 1567;
        int p_Total_NoofLates = 1568;
        int OT_Allocated_Leaves_ID = 177; // Object Type ALlocated Leaves



   

        [EventHandler(MFEventHandlerType.MFEventHandlerBeforeCreateNewObjectFinalize, Class = "CL.LeaveRecord")]
        [EventHandler(MFEventHandlerType.MFEventHandlerBeforeSetProperties, Class = "CL.LeaveRecord")]
        public void LeaveApplicationAuth(EventHandlerEnvironment env)
        {
            try
            {


                if (env.PropertyValues.GetProperty(p_Employee_Name) != null &&
                    env.PropertyValues.GetProperty(p_Employee_Code) != null &&
                    env.PropertyValues.GetProperty(p_Leave_Type) != null &&
                    env.PropertyValues.GetProperty(p_Start_Date) != null &&
                    env.PropertyValues.GetProperty(p_End_Date) != null)
                {
                    // Get Employee Name
                    var get_Employee_Name = env.PropertyValues.GetProperty(p_Employee_Name).Value.DisplayValue.ToString();

                    // Get Employee Code
                    var get_Employee_Code = env.PropertyValues.GetProperty(p_Employee_Code).Value.DisplayValue.ToString();


                    // Get Employee Leave Type
                    var get_Leave_Type = env.PropertyValues.GetProperty(p_Leave_Type).Value.DisplayValue.ToString();

                    // Get Leave Start Date
                    var get_Start_Date = env.PropertyValues.GetProperty(p_Start_Date).Value.DisplayValue.ToString();

                    // Get Leave Start Date
                    var get_End_Date = env.PropertyValues.GetProperty(p_End_Date).Value.DisplayValue.ToString();

                    double no_OF_Leave_Days = 0.0;

                    if (get_Leave_Type != "" && get_Start_Date != "" && get_End_Date != "")
                    {
                        // Get No oF leaves Days Applied
                        var startdate = Convert.ToDateTime(get_Start_Date).AddHours(0).AddMinutes(0);
                        var Enddate = Convert.ToDateTime(get_End_Date).AddHours(23).AddMinutes(59);

                       no_OF_Leave_Days = Convert.ToInt32((Enddate - startdate).TotalDays);

                        if (no_OF_Leave_Days < 0)
                            throw new Exception("Leaves Starting and Ending Date is Invalid");

                        // Get Employee Allocated Leaves
                        var searchBuilder = new MFSearchBuilder(env.Vault);

                        // Add an object type filter.
                        searchBuilder.ObjType(OT_Allocated_Leaves_ID);
                        searchBuilder.Property(p_Employee_Name, MFDataType.MFDatatypeText, get_Employee_Name);
                        searchBuilder.Property(p_Employee_Code, MFDataType.MFDatatypeText, get_Employee_Code);

                        // Add a "not deleted" filter.
                        searchBuilder.Deleted(false);

                        // Execute the search.
                        var AllocatedLeaves = searchBuilder.FindEx();
                        var ObjType = AllocatedLeaves[0].ObjVer.Type;
                        var ID = AllocatedLeaves[0].ObjVer.ID;

                        int LAV_ID = 0;
                        int LR_ID = 0;

                        double LAV_Value = 0;
                        double LR_Value = 0;


                        double leave_Can_Avail = 0;
                        switch (get_Leave_Type)
                        {

                            case "Casual Leaves":

                                LAV_ID = 1540;
                                LR_ID = 1544;

                                int Total_CL_ID = 1512;

                                double Total_CL_Allow = Convert.ToDouble(AllocatedLeaves[0].Properties.GetProperty(Total_CL_ID).Value.DisplayValue.ToString());

                                if (AllocatedLeaves[0].Properties.GetProperty(LAV_ID).Value.DisplayValue.ToString() == "" && AllocatedLeaves[0].Properties.GetProperty(LR_ID).Value.DisplayValue.ToString() == "")
                                {
                                    LAV_Value = 0;
                                    LR_Value = 0;

                                }
                                else
                                {
                                    LAV_Value = Convert.ToDouble(AllocatedLeaves[0].Properties.GetProperty(LAV_ID).Value.DisplayValue.ToString());
                                    LR_Value = Convert.ToDouble(AllocatedLeaves[0].Properties.GetProperty(LR_ID).Value.DisplayValue.ToString()); ;

                                }


                                leave_Can_Avail = Total_CL_Allow - LAV_Value;

                                if (no_OF_Leave_Days > leave_Can_Avail)
                                {
                                    throw new Exception("No of Casual Leaves You are Applying are Out of Limit.");
                                }

                                break;


                            case "Annual Leaves":

                                LAV_ID = 1541;
                                LR_ID = 1546;


                                int Total_AL_ID = 1514;

                                int Total_AL_Allow = Convert.ToInt32(AllocatedLeaves[0].Properties.GetProperty(Total_AL_ID).Value.DisplayValue.ToString());

                                if (AllocatedLeaves[0].Properties.GetProperty(LAV_ID).Value.DisplayValue.ToString() == "" && AllocatedLeaves[0].Properties.GetProperty(LR_ID).Value.DisplayValue.ToString() == "")
                                {
                                    LAV_Value = 0;
                                    LR_Value = 0;

                                }
                                else
                                {
                                    LAV_Value = Convert.ToDouble(AllocatedLeaves[0].Properties.GetProperty(LAV_ID).Value.DisplayValue.ToString());
                                    LR_Value = Convert.ToDouble(AllocatedLeaves[0].Properties.GetProperty(LR_ID).Value.DisplayValue.ToString()); ;

                                }

                                leave_Can_Avail = Total_AL_Allow - LAV_Value;

                                if (no_OF_Leave_Days > leave_Can_Avail)
                                {
                                    throw new Exception("No of Annual Leaves You are Applying are Out of Limit.");
                                }

                                break;


                            case "Sick Leaves":


                                LAV_ID = 1542;
                                LR_ID = 1545;



                                int Total_SL_ID = 1513;

                                int Total_SL_Allow = Convert.ToInt32(AllocatedLeaves[0].Properties.GetProperty(Total_SL_ID).Value.DisplayValue.ToString());

                                if (AllocatedLeaves[0].Properties.GetProperty(LAV_ID).Value.DisplayValue.ToString() == "" && AllocatedLeaves[0].Properties.GetProperty(LR_ID).Value.DisplayValue.ToString() == "")
                                {
                                    LAV_Value = 0;
                                    LR_Value = 0;

                                }
                                else
                                {
                                    LAV_Value = Convert.ToDouble(AllocatedLeaves[0].Properties.GetProperty(LAV_ID).Value.DisplayValue.ToString());
                                    LR_Value = Convert.ToDouble(AllocatedLeaves[0].Properties.GetProperty(LR_ID).Value.DisplayValue.ToString()); ;

                                }


                                leave_Can_Avail = Total_SL_Allow - LAV_Value;

                                if (no_OF_Leave_Days > leave_Can_Avail)
                                {
                                    throw new Exception("No of Sick Leaves You are Applying are Out of Limit.");
                                }


                                break;

                            case "Half Day":


                                LAV_ID = 1540;
                                LR_ID = 1544;


                                int Total_HL_ID = 1512;

                                int Total_HL_Allow = Convert.ToInt32(AllocatedLeaves[0].Properties.GetProperty(Total_HL_ID).Value.DisplayValue.ToString());

                                if (AllocatedLeaves[0].Properties.GetProperty(LAV_ID).Value.DisplayValue.ToString() == "" && AllocatedLeaves[0].Properties.GetProperty(LR_ID).Value.DisplayValue.ToString() == "")
                                {
                                    LAV_Value = 0;
                                    LR_Value = 0;

                                }
                                else
                                {
                                    LAV_Value = Convert.ToDouble(AllocatedLeaves[0].Properties.GetProperty(LAV_ID).Value.DisplayValue.ToString());
                                    LR_Value = Convert.ToDouble(AllocatedLeaves[0].Properties.GetProperty(LR_ID).Value.DisplayValue.ToString()); ;

                                }

                                leave_Can_Avail = Total_HL_Allow - LAV_Value;
                                no_OF_Leave_Days = 0.5;
                                if (no_OF_Leave_Days > leave_Can_Avail)
                                {
                                    throw new Exception("Half Day Can be applied on the same day.");
                                }



                                break;


                            case "Saturdays":


                                string IsSaturday = Convert.ToDateTime(get_Start_Date).DayOfWeek.ToString();

                                LAV_ID = 1549;
                                LR_ID = 1550;


                                int Total_SHRL_ID = 1551;

                                int Total_SHRL_Allow = Convert.ToInt32(AllocatedLeaves[0].Properties.GetProperty(Total_SHRL_ID).Value.DisplayValue.ToString());

                                if (AllocatedLeaves[0].Properties.GetProperty(LAV_ID).Value.DisplayValue.ToString() == "" && AllocatedLeaves[0].Properties.GetProperty(LR_ID).Value.DisplayValue.ToString() == "")
                                {
                                    LAV_Value = 0;
                                    LR_Value = 0;

                                }
                                else
                                {
                                    LAV_Value = Convert.ToDouble(AllocatedLeaves[0].Properties.GetProperty(LAV_ID).Value.DisplayValue.ToString());
                                    LR_Value = Convert.ToDouble(AllocatedLeaves[0].Properties.GetProperty(LR_ID).Value.DisplayValue.ToString()); ;

                                }

                                if (no_OF_Leave_Days > 1)
                                {
                                    throw new Exception("No of days are More the one day.");
                                }
                                if (IsSaturday.ToLower() != "saturday")
                                {
                                    throw new Exception("Selected Dates are not Saturday.");
                                }

                                break;


                            case "Unpaid Leave":


                                LAV_ID = 1543;
                                LR_ID = 1547;


                                int Total_UPL_ID = 1515;

                                int Total_UPRL_Allow = Convert.ToInt32(AllocatedLeaves[0].Properties.GetProperty(Total_UPL_ID).Value.DisplayValue.ToString());

                                if (AllocatedLeaves[0].Properties.GetProperty(LAV_ID).Value.DisplayValue.ToString() == "" && AllocatedLeaves[0].Properties.GetProperty(LR_ID).Value.DisplayValue.ToString() == "")
                                {
                                    LAV_Value = 0;
                                    LR_Value = 0;

                                }
                                else
                                {
                                    LAV_Value = Convert.ToDouble(AllocatedLeaves[0].Properties.GetProperty(LAV_ID).Value.DisplayValue.ToString());
                                    LR_Value = Convert.ToDouble(AllocatedLeaves[0].Properties.GetProperty(LR_ID).Value.DisplayValue.ToString()); ;

                                }
                                break;

                            default:
                                break;
                        }


                    }
                    else
                    {
                        if (get_Leave_Type == "")
                            throw new Exception("Leave Type Not Selected.");
                        if (get_Start_Date == "" || get_End_Date == "")
                            throw new Exception("Leaves Start or End Date Not Provided.");

                    }


                    //var objID = new MFilesAPI.ObjID();
                    //objID.SetIDs(
                    //    ObjType: env.ObjVer.Type,
                    //    ID: env.ObjVer.ID);

                    //// Check out the object.
                    //var checkedOutObjectVersion = env.Vault.ObjectOperations.CheckOut(objID);

                    //// Create a property value to update.
                    //var LeavesApplied = new MFilesAPI.PropertyValue
                    //{
                    //    PropertyDef = NoofLeavesRequested
                    //};
                    //LeavesApplied.Value.SetValue(
                    //    MFDataType.MFDatatypeFloating,  // This must be correct for the property definition.
                    //    no_OF_Leave_Days
                    //);

                    //env.Vault.ObjectPropertyOperations.SetProperty(
                    // ObjVer: checkedOutObjectVersion.ObjVer,
                    // PropertyValue: LeavesApplied);

                    //// Check the object back in.
                    //env.Vault.ObjectOperations.CheckIn(checkedOutObjectVersion.ObjVer);


                }




            }
            catch (System.Exception ex)
            {

                throw new Exception(ex.Message);
            }

        }

        

        [StateAction("WFS.LeaveProcess.Manager")]
        public void CheckForDocumentInSickLeave(StateEnvironment env)
        {
            // Get Employee Leave Type
            var get_Leave_Type = env.PropertyValues.GetProperty(p_Leave_Type).Value.DisplayValue.ToString();

            // Get Leave Start Date
            var get_Start_Date = env.PropertyValues.GetProperty(p_Start_Date).Value.DisplayValue.ToString();

            // Get Leave Start Date
            var get_End_Date = env.PropertyValues.GetProperty(p_End_Date).Value.DisplayValue.ToString();

            // Get No oF leaves Days Applied
            int no_OF_Leave_Days = Convert.ToInt32((Convert.ToDateTime(get_End_Date) - Convert.ToDateTime(get_Start_Date)).TotalDays);


            if (get_Leave_Type == "Sick Leaves" && no_OF_Leave_Days > 2)
            {
                var HasFiles = env.ObjVerEx.Info.FilesCount;

                if (HasFiles == 0)
                {
                    throw new Exception("Medical Document is Required For Sick Leaves More Than 2 Days.");

                }

            }


        }


        [StateAction("WFS.LeaveProcess.Saturdayapproved")]
        public void UpdateEmployeeSarurdayLeaves(StateEnvironment env)
        {
            try
            {
                // Get Employee Name
                var get_Employee_Name = env.PropertyValues.GetProperty(p_Employee_Name).Value.DisplayValue.ToString();

                // Get Employee Code
                var get_Employee_Code = env.PropertyValues.GetProperty(p_Employee_Code).Value.DisplayValue.ToString();

                // Get Employee Leave Type
                var get_Leave_Type = env.PropertyValues.GetProperty(p_Leave_Type).Value.DisplayValue.ToString();

                if (get_Leave_Type != "Saturdays")
                {
                    throw new Exception("This State is Only For Saturdays Leaves");
                }

                // Get Leave Start Date
                var get_Start_Date = env.PropertyValues.GetProperty(p_Start_Date).Value.DisplayValue.ToString();

                // Get Leave Start Date
                var get_End_Date = env.PropertyValues.GetProperty(p_End_Date).Value.DisplayValue.ToString();

                // Get No oF leaves Days Applied
                var startdate = Convert.ToDateTime(get_Start_Date).AddHours(0).AddMinutes(0);
                var Enddate = Convert.ToDateTime(get_End_Date).AddHours(23).AddMinutes(59);

                int no_OF_Leave_Days = Convert.ToInt32((Enddate - startdate).TotalDays);

                var searchBuilder = new MFSearchBuilder(env.Vault);

                // Add an object type filter.
                searchBuilder.ObjType(OT_Allocated_Leaves_ID);
                searchBuilder.Property(p_Employee_Name, MFDataType.MFDatatypeText, get_Employee_Name);
                searchBuilder.Property(p_Employee_Code, MFDataType.MFDatatypeText, get_Employee_Code);


                // Add a "not deleted" filter.
                searchBuilder.Deleted(false);

                // Execute the search.
                var AllocatedLeaves = searchBuilder.FindEx();
                var ObjType = AllocatedLeaves[0].ObjVer.Type;
                var ID = AllocatedLeaves[0].ObjVer.ID;

                int LAV_ID = 0;
                int LR_ID = 0;

                double LAV_Value = 0;
                double LR_Value = 0;


                switch (get_Leave_Type)
                {

                    case "Saturdays":

                        LAV_ID = 1549;
                        LR_ID = 1550;

                        int Total_SatRL_ID = 1551;

                        int Total_SatRL_Allow = Convert.ToInt32(AllocatedLeaves[0].Properties.GetProperty(Total_SatRL_ID).Value.DisplayValue.ToString());

                        if (AllocatedLeaves[0].Properties.GetProperty(LAV_ID).Value.DisplayValue.ToString() == "" && AllocatedLeaves[0].Properties.GetProperty(LR_ID).Value.DisplayValue.ToString() == "")
                        {
                            LAV_Value = 0;
                            LR_Value = 0;

                        }
                        else
                        {
                            LAV_Value = Convert.ToDouble(AllocatedLeaves[0].Properties.GetProperty(LAV_ID).Value.DisplayValue.ToString());
                            LR_Value = Convert.ToDouble(AllocatedLeaves[0].Properties.GetProperty(LR_ID).Value.DisplayValue.ToString()); ;

                        }
                        no_OF_Leave_Days = 1;
                        LR_Value = Total_SatRL_Allow - LAV_Value - no_OF_Leave_Days;
                        LAV_Value += no_OF_Leave_Days;


                        break;

                    default:


                        break;
                }




                // Updating LEaves 

                // We want to alter the document with ID 249.
                var objID = new MFilesAPI.ObjID();
                objID.SetIDs(
                    ObjType: ObjType,
                    ID: ID);

                // Check out the object.
                var checkedOutObjectVersion = AllocatedLeaves[0].Vault.ObjectOperations.CheckOut(objID);

                // Create a property value to update.
                var LeavesAvailed = new MFilesAPI.PropertyValue
                {
                    PropertyDef = LAV_ID
                };
                LeavesAvailed.Value.SetValue(
                    MFDataType.MFDatatypeFloating,  // This must be correct for the property definition.
                    LAV_Value
                );

                var LeavesRemaining = new MFilesAPI.PropertyValue
                {
                    PropertyDef = LR_ID
                };
                LeavesRemaining.Value.SetValue(
                    MFDataType.MFDatatypeFloating,  // This must be correct for the property definition.
                    LR_Value
                );

                // Update the property on the server.
                env.Vault.ObjectPropertyOperations.SetProperty(
                    ObjVer: checkedOutObjectVersion.ObjVer,
                    PropertyValue: LeavesAvailed);

                env.Vault.ObjectPropertyOperations.SetProperty(
                  ObjVer: checkedOutObjectVersion.ObjVer,
                  PropertyValue: LeavesRemaining);

                // Check the object back in.
                env.Vault.ObjectOperations.CheckIn(checkedOutObjectVersion.ObjVer);

            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }
        }

        [StateAction("WFS.LeaveProcess.Approve")]
        public void RestrictingSarurdayLeavesonManagerApprove(StateEnvironment env)
        {
            try
            {

                // Get Employee Leave Type
                var get_Leave_Type = env.PropertyValues.GetProperty(p_Leave_Type).Value.DisplayValue.ToString();

                if (get_Leave_Type == "Saturdays")
                {
                    throw new Exception("For Saturdays Leaves This State Is not Allowed.");
                }


            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }
        }

        [StateAction("WFS.LeaveProcess.ApproveByCeo")]
        [StateAction("WFS.LeaveProcess.ApprovedByHR")]
        public void UpdateEmployeeLeaves(StateEnvironment env)
        {
            try
            {
                // Get Employee Name
                var get_Employee_Name = env.PropertyValues.GetProperty(p_Employee_Name).Value.DisplayValue.ToString();

                // Get Employee Code
                var get_Employee_Code = env.PropertyValues.GetProperty(p_Employee_Code).Value.DisplayValue.ToString();

                // Get Employee Leave Type
                var get_Leave_Type = env.PropertyValues.GetProperty(p_Leave_Type).Value.DisplayValue.ToString();

                // Get Leave Start Date
                var get_Start_Date = env.PropertyValues.GetProperty(p_Start_Date).Value.DisplayValue.ToString();

                // Get Leave Start Date
                var get_End_Date = env.PropertyValues.GetProperty(p_End_Date).Value.DisplayValue.ToString();

                // Get No oF leaves Days Applied
                var startdate = Convert.ToDateTime(get_Start_Date).AddHours(0).AddMinutes(0);
                var Enddate = Convert.ToDateTime(get_End_Date).AddHours(23).AddMinutes(59);

                double no_OF_Leave_Days = Convert.ToInt32((Enddate - startdate).TotalDays);

               // int NoOfSundays = CheckSundays(startdate, Enddate, no_OF_Leave_Days);

                // Get Employee Allocated Leaves
                //no_OF_Leave_Days = no_OF_Leave_Days - NoOfSundays;

                var searchBuilder = new MFSearchBuilder(env.Vault);

                // Add an object type filter.
                searchBuilder.ObjType(OT_Allocated_Leaves_ID);
                searchBuilder.Property(p_Employee_Name, MFDataType.MFDatatypeText, get_Employee_Name);
                searchBuilder.Property(p_Employee_Code, MFDataType.MFDatatypeText, get_Employee_Code);


                // Add a "not deleted" filter.
                searchBuilder.Deleted(false);

                // Execute the search.
                var AllocatedLeaves = searchBuilder.FindEx();
                var ObjType = AllocatedLeaves[0].ObjVer.Type;
                var ID = AllocatedLeaves[0].ObjVer.ID;

                int LAV_ID = 0;
                int LR_ID = 0;

                double LAV_Value = 0;
                double LR_Value = 0;


                switch (get_Leave_Type)
                {
                    case "Casual Leaves":

                        LAV_ID = 1540;
                        LR_ID = 1544;

                        int Total_CL_ID = 1512;

                        double Total_CL_Allow = Convert.ToDouble(AllocatedLeaves[0].Properties.GetProperty(Total_CL_ID).Value.DisplayValue.ToString());

                        if (AllocatedLeaves[0].Properties.GetProperty(LAV_ID).Value.DisplayValue.ToString() == "" && AllocatedLeaves[0].Properties.GetProperty(LR_ID).Value.DisplayValue.ToString() == "")
                        {
                            LAV_Value = 0;
                            LR_Value = 0;

                        }
                        else
                        {
                            LAV_Value = Convert.ToDouble(AllocatedLeaves[0].Properties.GetProperty(LAV_ID).Value.DisplayValue.ToString());
                            LR_Value = Convert.ToDouble(AllocatedLeaves[0].Properties.GetProperty(LR_ID).Value.DisplayValue.ToString()); ;

                        }


                        LR_Value = Total_CL_Allow - LAV_Value - no_OF_Leave_Days;
                        LAV_Value += no_OF_Leave_Days;

                        break;


                    case "Annual Leaves":

                        LAV_ID = 1541;
                        LR_ID = 1546;


                        int Total_AL_ID = 1514;

                        int Total_AL_Allow = Convert.ToInt32(AllocatedLeaves[0].Properties.GetProperty(Total_AL_ID).Value.DisplayValue.ToString());

                        if (AllocatedLeaves[0].Properties.GetProperty(LAV_ID).Value.DisplayValue.ToString() == "" && AllocatedLeaves[0].Properties.GetProperty(LR_ID).Value.DisplayValue.ToString() == "")
                        {
                            LAV_Value = 0;
                            LR_Value = 0;

                        }
                        else
                        {
                            LAV_Value = Convert.ToDouble(AllocatedLeaves[0].Properties.GetProperty(LAV_ID).Value.DisplayValue.ToString());
                            LR_Value = Convert.ToDouble(AllocatedLeaves[0].Properties.GetProperty(LR_ID).Value.DisplayValue.ToString()); ;

                        }

                        LR_Value = Total_AL_Allow - LAV_Value - no_OF_Leave_Days;
                        LAV_Value += no_OF_Leave_Days;


                        break;


                    case "Sick Leaves":


                        LAV_ID = 1542;
                        LR_ID = 1545;



                        int Total_SL_ID = 1513;

                        int Total_SL_Allow = Convert.ToInt32(AllocatedLeaves[0].Properties.GetProperty(Total_SL_ID).Value.DisplayValue.ToString());

                        if (AllocatedLeaves[0].Properties.GetProperty(LAV_ID).Value.DisplayValue.ToString() == "" && AllocatedLeaves[0].Properties.GetProperty(LR_ID).Value.DisplayValue.ToString() == "")
                        {
                            LAV_Value = 0;
                            LR_Value = 0;

                        }
                        else
                        {
                            LAV_Value = Convert.ToDouble(AllocatedLeaves[0].Properties.GetProperty(LAV_ID).Value.DisplayValue.ToString());
                            LR_Value = Convert.ToDouble(AllocatedLeaves[0].Properties.GetProperty(LR_ID).Value.DisplayValue.ToString()); ;

                        }

                        LR_Value = Total_SL_Allow - LAV_Value - no_OF_Leave_Days;
                        LAV_Value += no_OF_Leave_Days;



                        break;

                    case "Half Day":


                        LAV_ID = 1540;
                        LR_ID = 1544;

                        Total_CL_ID = 1512;

                        double Total_HL_Allow = Convert.ToDouble(AllocatedLeaves[0].Properties.GetProperty(Total_CL_ID).Value.DisplayValue.ToString());

                        if (AllocatedLeaves[0].Properties.GetProperty(LAV_ID).Value.DisplayValue.ToString() == "" && AllocatedLeaves[0].Properties.GetProperty(LR_ID).Value.DisplayValue.ToString() == "")
                        {
                            LAV_Value = 0;
                            LR_Value = 0;

                        }
                        else
                        {
                            LAV_Value = Convert.ToDouble(AllocatedLeaves[0].Properties.GetProperty(LAV_ID).Value.DisplayValue.ToString());
                            LR_Value = Convert.ToDouble(AllocatedLeaves[0].Properties.GetProperty(LR_ID).Value.DisplayValue.ToString()); ;

                        }
                        no_OF_Leave_Days = 0.5;
                        LR_Value = Total_HL_Allow - LAV_Value - no_OF_Leave_Days;
                        LAV_Value += no_OF_Leave_Days;



                        break;


                    case "Saturdays":


                        LAV_ID = 1549;
                        LR_ID = 1550;


                        int Total_SatRL_ID = 1551;

                        int Total_SatRL_Allow = Convert.ToInt32(AllocatedLeaves[0].Properties.GetProperty(Total_SatRL_ID).Value.DisplayValue.ToString());

                        if (AllocatedLeaves[0].Properties.GetProperty(LAV_ID).Value.DisplayValue.ToString() == "" && AllocatedLeaves[0].Properties.GetProperty(LR_ID).Value.DisplayValue.ToString() == "")
                        {
                            LAV_Value = 0;
                            LR_Value = 0;

                        }
                        else
                        {
                            LAV_Value = Convert.ToDouble(AllocatedLeaves[0].Properties.GetProperty(LAV_ID).Value.DisplayValue.ToString());
                            LR_Value = Convert.ToDouble(AllocatedLeaves[0].Properties.GetProperty(LR_ID).Value.DisplayValue.ToString()); ;

                        }
                        no_OF_Leave_Days = 1;
                        LR_Value = Total_SatRL_Allow - LAV_Value - no_OF_Leave_Days;
                        LAV_Value += no_OF_Leave_Days;


                        break;


                    case "Unpaid Leave":


                        LAV_ID = 1543;
                        LR_ID = 1547;


                        int Total_UPL_ID = 1515;

                        int Total_UPRL_Allow = Convert.ToInt32(AllocatedLeaves[0].Properties.GetProperty(Total_UPL_ID).Value.DisplayValue.ToString());

                        if (AllocatedLeaves[0].Properties.GetProperty(LAV_ID).Value.DisplayValue.ToString() == "" && AllocatedLeaves[0].Properties.GetProperty(LR_ID).Value.DisplayValue.ToString() == "")
                        {
                            LAV_Value = 0;
                            LR_Value = 0;

                        }
                        else
                        {
                            LAV_Value = Convert.ToDouble(AllocatedLeaves[0].Properties.GetProperty(LAV_ID).Value.DisplayValue.ToString());
                            LR_Value = Convert.ToDouble(AllocatedLeaves[0].Properties.GetProperty(LR_ID).Value.DisplayValue.ToString()); ;

                        }

                        LR_Value = Total_UPRL_Allow - LAV_Value - no_OF_Leave_Days;
                        LAV_Value += no_OF_Leave_Days;


                        break;






                    default:
                        break;
                }




                // Updating LEaves 

                // We want to alter the document with ID 249.
                var objID = new MFilesAPI.ObjID();
                objID.SetIDs(
                    ObjType: ObjType,
                    ID: ID);

                // Check out the object.
                var checkedOutObjectVersion = AllocatedLeaves[0].Vault.ObjectOperations.CheckOut(objID);

                // Create a property value to update.
                var LeavesAvailed = new MFilesAPI.PropertyValue
                {
                    PropertyDef = LAV_ID
                };
                LeavesAvailed.Value.SetValue(
                    MFDataType.MFDatatypeFloating,  // This must be correct for the property definition.
                    LAV_Value
                );

                var LeavesRemaining = new MFilesAPI.PropertyValue
                {
                    PropertyDef = LR_ID
                };
                LeavesRemaining.Value.SetValue(
                    MFDataType.MFDatatypeFloating,  // This must be correct for the property definition.
                    LR_Value
                );

                // Update the property on the server.
                env.Vault.ObjectPropertyOperations.SetProperty(
                    ObjVer: checkedOutObjectVersion.ObjVer,
                    PropertyValue: LeavesAvailed);

                env.Vault.ObjectPropertyOperations.SetProperty(
                  ObjVer: checkedOutObjectVersion.ObjVer,
                  PropertyValue: LeavesRemaining);

                // Check the object back in.
                env.Vault.ObjectOperations.CheckIn(checkedOutObjectVersion.ObjVer);

            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message + " " + ex.StackTrace);
            }
        }


        public int CheckSundays(DateTime startDate, DateTime endDate, double Days)
        {
            int Sundays = 0;
            try
            {
                for (int index = 0; index < int.Parse(Days.ToString()); index++)
                {
                    DateTime dt = startDate.AddDays(index);
                    if (dt <= endDate)
                    {
                        if (dt.DayOfWeek == DayOfWeek.Sunday)
                            Sundays++;
                    }

                }
            }
            catch (Exception ex)
            {


            }

            return Sundays;
        }


        //////
        ///


        [EventHandler(MFEventHandlerType.MFEventHandlerBeforeCreateNewObjectFinalize, Class = "CL.EmployeeLates")]
        [EventHandler(MFEventHandlerType.MFEventHandlerBeforeSetProperties, Class = "CL.EmployeeLates")]

        public void LateLeaveApplication(EventHandlerEnvironment env)
        {
            try
            {


                if (env.PropertyValues.GetProperty(p_Employee_Name) != null &&
                    env.PropertyValues.GetProperty(p_Employee_Code) != null &&
                    env.PropertyValues.GetProperty(p_Adjust_Lates_To) != null)

                {
                    // Get Employee Name
                    var get_Employee_Name = env.PropertyValues.GetProperty(p_Employee_Name).Value.DisplayValue.ToString();

                    // Get Employee Code
                    var get_Employee_Code = env.PropertyValues.GetProperty(p_Employee_Code).Value.DisplayValue.ToString();


                    // Get Employee Leave Type
                    //var get_Late_Type = env.PropertyValues.GetProperty(p_Leave_Type).Value.DisplayValue.ToString();


                    var Adjust_Lates_To = env.PropertyValues.GetProperty(p_Adjust_Lates_To).Value.DisplayValue.ToString();
                    double no_OF_Lates = Convert.ToDouble(env.PropertyValues.GetProperty(1567).Value.DisplayValue.ToString());

                    if (no_OF_Lates == 0)
                    {
                        throw new Exception("Number Of Lates Enter Must Be Greater Than Zero.");
                    }

                    if (Adjust_Lates_To != "")
                    {
                        // Get No oF leaves Days Applied


                        if (no_OF_Lates == 0)
                            throw new Exception("Enter Number of Lates");

                        // Get Employee Allocated Leaves
                        var searchBuilder = new MFSearchBuilder(env.Vault);

                        // Add an object type filter.
                        searchBuilder.ObjType(OT_Allocated_Leaves_ID);
                        searchBuilder.Property(p_Employee_Name, MFDataType.MFDatatypeText, get_Employee_Name);
                        searchBuilder.Property(p_Employee_Code, MFDataType.MFDatatypeText, get_Employee_Code);

                        // Add a "not deleted" filter.
                        searchBuilder.Deleted(false);

                        // Execute the search.
                        var AllocatedLeaves = searchBuilder.FindEx();
                        var ObjType = AllocatedLeaves[0].ObjVer.Type;
                        var ID = AllocatedLeaves[0].ObjVer.ID;

                        int LAV_ID = 0;
                        int LR_ID = 0;

                        double LAV_Value = 0;
                        double LR_Value = 0;


                        double leave_Can_Avail = 0;
                        
                        var no_OF_Leave_Days = no_OF_Lates;
                        

                        switch (Adjust_Lates_To)
                        {
                            case "Casual Leaves":

                                LAV_ID = 1540;
                                LR_ID = 1544;

                                int Total_CL_ID = 1512;

                                double Total_CL_Allow = Convert.ToDouble(AllocatedLeaves[0].Properties.GetProperty(Total_CL_ID).Value.DisplayValue.ToString());

                                if (AllocatedLeaves[0].Properties.GetProperty(LAV_ID).Value.DisplayValue.ToString() == "" && AllocatedLeaves[0].Properties.GetProperty(LR_ID).Value.DisplayValue.ToString() == "")
                                {
                                    LAV_Value = 0;
                                    LR_Value = 0;

                                }
                                else
                                {
                                    LAV_Value = Convert.ToDouble(AllocatedLeaves[0].Properties.GetProperty(LAV_ID).Value.DisplayValue.ToString());
                                    LR_Value = Convert.ToDouble(AllocatedLeaves[0].Properties.GetProperty(LR_ID).Value.DisplayValue.ToString()); ;

                                }


                                leave_Can_Avail = Total_CL_Allow - LAV_Value;

                                if (no_OF_Leave_Days > leave_Can_Avail)
                                {
                                    throw new Exception("No of Casual Leaves You are Applying are Out of Limit.");
                                }

                                LR_Value = Total_CL_Allow - LAV_Value - no_OF_Leave_Days;
                                LAV_Value += no_OF_Leave_Days;

                                break;


                            case "Annual Leaves":

                                LAV_ID = 1541;
                                LR_ID = 1546;


                                int Total_AL_ID = 1514;

                                int Total_AL_Allow = Convert.ToInt32(AllocatedLeaves[0].Properties.GetProperty(Total_AL_ID).Value.DisplayValue.ToString());

                                if (AllocatedLeaves[0].Properties.GetProperty(LAV_ID).Value.DisplayValue.ToString() == "" && AllocatedLeaves[0].Properties.GetProperty(LR_ID).Value.DisplayValue.ToString() == "")
                                {
                                    LAV_Value = 0;
                                    LR_Value = 0;

                                }
                                else
                                {
                                    LAV_Value = Convert.ToDouble(AllocatedLeaves[0].Properties.GetProperty(LAV_ID).Value.DisplayValue.ToString());
                                    LR_Value = Convert.ToDouble(AllocatedLeaves[0].Properties.GetProperty(LR_ID).Value.DisplayValue.ToString()); ;

                                }

                                leave_Can_Avail = Total_AL_Allow - LAV_Value;

                                if (no_OF_Leave_Days > leave_Can_Avail)
                                {
                                    throw new Exception("No of Annual Leaves You are Applying are Out of Limit.");
                                }

                                LR_Value = Total_AL_Allow - LAV_Value - no_OF_Leave_Days;
                                LAV_Value += no_OF_Leave_Days;


                                break;


                            case "Sick Leaves":


                                LAV_ID = 1542;
                                LR_ID = 1545;



                                int Total_SL_ID = 1513;

                                int Total_SL_Allow = Convert.ToInt32(AllocatedLeaves[0].Properties.GetProperty(Total_SL_ID).Value.DisplayValue.ToString());

                                if (AllocatedLeaves[0].Properties.GetProperty(LAV_ID).Value.DisplayValue.ToString() == "" && AllocatedLeaves[0].Properties.GetProperty(LR_ID).Value.DisplayValue.ToString() == "")
                                {
                                    LAV_Value = 0;
                                    LR_Value = 0;

                                }
                                else
                                {
                                    LAV_Value = Convert.ToDouble(AllocatedLeaves[0].Properties.GetProperty(LAV_ID).Value.DisplayValue.ToString());
                                    LR_Value = Convert.ToDouble(AllocatedLeaves[0].Properties.GetProperty(LR_ID).Value.DisplayValue.ToString()); ;

                                }

                                leave_Can_Avail = Total_SL_Allow - LAV_Value;

                                if (no_OF_Leave_Days > leave_Can_Avail)
                                {
                                    throw new Exception("No of Sick Leaves You are Applying are Out of Limit.");
                                }

                                LR_Value = Total_SL_Allow - LAV_Value - no_OF_Leave_Days;
                                LAV_Value += no_OF_Leave_Days;



                                break;

                            
                            case "Unpaid Leave":


                                LAV_ID = 1543;
                                LR_ID = 1547;


                                int Total_UPL_ID = 1515;

                                int Total_UPRL_Allow = Convert.ToInt32(AllocatedLeaves[0].Properties.GetProperty(Total_UPL_ID).Value.DisplayValue.ToString());

                                if (AllocatedLeaves[0].Properties.GetProperty(LAV_ID).Value.DisplayValue.ToString() == "" && AllocatedLeaves[0].Properties.GetProperty(LR_ID).Value.DisplayValue.ToString() == "")
                                {
                                    LAV_Value = 0;
                                    LR_Value = 0;

                                }
                                else
                                {
                                    LAV_Value = Convert.ToDouble(AllocatedLeaves[0].Properties.GetProperty(LAV_ID).Value.DisplayValue.ToString());
                                    LR_Value = Convert.ToDouble(AllocatedLeaves[0].Properties.GetProperty(LR_ID).Value.DisplayValue.ToString()); ;

                                }

                                leave_Can_Avail = Total_UPRL_Allow - LAV_Value;

                                if (no_OF_Leave_Days > leave_Can_Avail)
                                {
                                    throw new Exception("No of Unpaid Leaves You are Applying are Out of Limit.");
                                }

                                LR_Value = Total_UPRL_Allow - LAV_Value - no_OF_Leave_Days;
                                LAV_Value += no_OF_Leave_Days;


                                break;


                            default:
                                throw new Exception("Leave Adjust To Not Selected.");
                               
                        }

                        double Total_Lates_Availed = 0;
                        // Overall Total Noof Lates done
                        if (AllocatedLeaves[0].Properties.GetProperty(p_Total_NoofLates).Value.DisplayValue.ToString() != "")
                        {
                         Total_Lates_Availed = Convert.ToDouble(AllocatedLeaves[0].Properties.GetProperty(p_Total_NoofLates).Value.DisplayValue.ToString());
                            
                        }
                        

                        Total_Lates_Availed = Total_Lates_Availed + no_OF_Leave_Days;
                        // Updating LEaves 

                        // We want to alter the document with ID 249.
                        var objID = new MFilesAPI.ObjID();
                        objID.SetIDs(
                            ObjType: ObjType,
                            ID: ID);

                        // Check out the object.
                        var checkedOutObjectVersion = AllocatedLeaves[0].Vault.ObjectOperations.CheckOut(objID);

                        // Create a property value to update.
                        var LeavesAvailed = new MFilesAPI.PropertyValue
                        {
                            PropertyDef = LAV_ID
                        };
                        LeavesAvailed.Value.SetValue(
                            MFDataType.MFDatatypeFloating,  // This must be correct for the property definition.
                            LAV_Value
                        );

                        var LeavesRemaining = new MFilesAPI.PropertyValue
                        {
                            PropertyDef = LR_ID
                        };
                        LeavesRemaining.Value.SetValue(
                            MFDataType.MFDatatypeFloating,  // This must be correct for the property definition.
                            LR_Value
                        );

                        // Setting Up For Total No OF Lates Done

                        var Total_Lates = new MFilesAPI.PropertyValue
                        {
                            PropertyDef =p_Total_NoofLates
                        };
                        Total_Lates.Value.SetValue(
                            MFDataType.MFDatatypeInteger,  // This must be correct for the property definition.
                           Total_Lates_Availed
                        );


                        // Setting Up For  No OF Lates Done this montly
                        var Lates_Per_Month = new MFilesAPI.PropertyValue
                        {
                            PropertyDef = p_NoofLates_monthly
                        };
                        Lates_Per_Month.Value.SetValue(
                            MFDataType.MFDatatypeInteger,  // This must be correct for the property definition.
                            no_OF_Leave_Days
                        );



                        // Update the property on the server.
                        env.Vault.ObjectPropertyOperations.SetProperty(
                            ObjVer: checkedOutObjectVersion.ObjVer,
                            PropertyValue: LeavesAvailed);

                        env.Vault.ObjectPropertyOperations.SetProperty(
                          ObjVer: checkedOutObjectVersion.ObjVer,
                          PropertyValue: LeavesRemaining);

                        // Setting Up For Total No OF Lates Done
                        env.Vault.ObjectPropertyOperations.SetProperty(
                           ObjVer: checkedOutObjectVersion.ObjVer,
                           PropertyValue: Total_Lates);

                        // Setting Up For  No OF Lates Done this montly
                        env.Vault.ObjectPropertyOperations.SetProperty(
                           ObjVer: checkedOutObjectVersion.ObjVer,
                           PropertyValue: Lates_Per_Month);

                        // Check the object back in.
                        env.Vault.ObjectOperations.CheckIn(checkedOutObjectVersion.ObjVer);
                    }
                    else
                    {
                        if (Adjust_Lates_To == "")
                            throw new Exception("Leave Type Not Selected.");

                    }




                }




            }
            catch (System.Exception ex)
            {

                throw new Exception(ex.Message);
            }

        }

    }
}




