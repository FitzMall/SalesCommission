
<!--	#include virtual = "/asp/security.asp" -->
<!--#include virtual="/check.asp"-->
<!--#include virtual="/inc/sendCDOemail.asp"-->
<%

IF ucase(trim(application("RIMASTLOCK"))) = "YES" then
    response.write "PRICE MATRIX Temporarily disabled for website updates !!!  BE BACK SHORTLY!! "
    response.end
end if
%>

<%
' ====  BPP COST ===
DIM BPPCOST
' $495 on 6/1/2011 (DO NOT CHANGE THIS VALUE - NO WAY - NO HOW!!!
BPPCOST = application("BPPprice")
%>


<%

Response.Buffer = true
'global variables
g_dbDBC = "Rimast"
g_dbTABLE = "Rimast"
g_dbWHfld = ""
g_dbWHoper = ""
g_dbWHval = ""
g_dbORfld = "model"
g_glblSQL = ""
g_loc = ""
g_id = ""
PageNumber = 0
TotalPages = 0
conn = ""
rs = ""
glblerr = 0
g_allowedLocations = "LFO,LFT,CJE,FBS,CDO,FMM,FOC,FLP,FCG,FTO"   '"FBS,LFO,CJE,CDO,LFT"
g_disallowedMakeCodes = "OL,PN,GC,BU,VP"
g_allowedModels = ""
g_enableLeasing = "false"
%>
<%
'##############################################################
'##################Update The Incentives#######################

SUB SaveIncentives()

	dim conn,rsInApp,sqlInApp,i,arrIncenvs
	Set conn = Server.CreateObject("ADODB.Connection")
	conn.Open Application("RACKSPACEDB")

	Set Rs = Server.CreateObject("ADODB.Recordset")
	sqlInApp = "DELETE FROM incentivesApplied WHERE ia_ModelNo = '" & session("tm0_3")&"'"

	Rs.open sqlInApp,conn,1,3
	set Rs=nothing


	arrIncenvs = split(request.form("chkApplied"),",")

	Set Rs = Server.CreateObject("ADODB.Recordset")
	Rs.Open "incentivesApplied", conn,1,3

	for i = 0 to ubound(arrIncenvs)
	 	Rs.AddNew
		Rs("ia_ModelNo")=session("tm0_3")
		Rs("ia_InKey")=arrIncenvs(i)
	 	Rs.Update
	next

	Rs.close
	set Rs=nothing

END SUB
'##############################################################
'##################Update The Incentives#######################
%>
<!-- #include virtual = "/asp/secure.asp" -->
<!-- #include file = "adovbs.inc" -->
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 3.2//EN">
<html>
<head>
<title>Year  Make ModelPackageCarline</title>

<SCRIPT LANGUAGE="JavaScript" src="dateChk.js"></script>

<SCRIPT LANGUAGE="JavaScript" src="rate.js"></script>

<meta name="GENERATOR" content="Namo WebEditor v5.0">
<script language="JavaScript">
function popUp(page) {
so=eval("window.open('"+page+"','so','toolbar=0,scrollbars=1,location=0,status=0,menubars=0,resizable=1,width=340,height=350')")
so.focus();
}
</script>
<script language="JavaScript">
<!--
function submitForm(frm,actionURL){

	frm.action = actionURL;
	frm.method = "post";
	frm.submit();
};
//-->
</script>

<script>
   function updateTextBoxes(value) {

     
       if(value == '10K') {
           document.getElementById("cb5_4").value = '0.00';
           document.getElementById("cb5_4").disabled = true;

           document.getElementById("cb5_6").value = '0.00';
           document.getElementById("cb5_6").disabled = true;

           document.getElementById("cb5_5").disabled = false;
       }
       else if(value == '12K') {
           document.getElementById("cb5_4").value = '0.00';
           document.getElementById("cb5_4").disabled = true;

           document.getElementById("cb5_5").value = '0.00';
           document.getElementById("cb5_5").disabled = true;

           document.getElementById("cb5_6").disabled = false;
       }
       else if(value == '15K') {
           document.getElementById("cb5_5").value = '0.00';
           document.getElementById("cb5_5").disabled = true;

           document.getElementById("cb5_6").value = '0.00';
           document.getElementById("cb5_6").disabled = true;

           document.getElementById("cb5_4").disabled = false;
       }

	var disclaimer;
	disclaimer = document.getElementById("cb5_10").value;

	start = disclaimer.indexOf(",");
	end = disclaimer.indexOf("miles",start);

	disclaimer = disclaimer.replace(disclaimer.substring(start,end),", " + value.replace("K",",000") + " ");

	document.getElementById("cb5_10").value = disclaimer;


   }

	function updateDisclaimerExcessMileage(value) {

		var disclaimer;
		disclaimer = document.getElementById("cb5_10").value;

		start = disclaimer.indexOf("end.");
		end = disclaimer.indexOf("per",start);

		disclaimer = disclaimer.replace(disclaimer.substring(start,end),"end. $" + value + " ");

		document.getElementById("cb5_10").value = disclaimer;

	}

	function updateDisclaimerDispositionFee(value) {

		var disclaimer;
		disclaimer = document.getElementById("cb5_10").value;

		start = disclaimer.indexOf("include");
		end = disclaimer.indexOf("disposition",start);

		disclaimer = disclaimer.replace(disclaimer.substring(start,end),"include $" + value + " ");

		document.getElementById("cb5_10").value = disclaimer;

	}

	function updateDisclaimerDeliveryDate(value) {

		var disclaimer;
		disclaimer = document.getElementById("cb5_10").value;

		start = disclaimer.indexOf("before");
		end = disclaimer.indexOf(".",start);

		disclaimer = disclaimer.replace(disclaimer.substring(start,end),"before " + value + "");

		document.getElementById("cb5_10").value = disclaimer;

	}

	function updateDisclaimerDownPayment(value) {

		var disclaimer;
		disclaimer = document.getElementById("cb5_10").value;

		start = disclaimer.indexOf("year, ");
		end = disclaimer.indexOf("down",start);

		disclaimer = disclaimer.replace(disclaimer.substring(start,end),"year, $" + value + " ");

		document.getElementById("cb5_10").value = disclaimer;

	}

	function updateRateInDisclaimer()
	{
		var value;
		value = document.getElementById("cb5_1").value;

		updateDisclaimerRate(value);
	}

	function updateDisclaimerRate(value) {

		var disclaimer;
		disclaimer = document.getElementById("cb5_10").value;

		start = disclaimer.indexOf("of");
		end = disclaimer.indexOf("for",start);

		var bInterestRate;
		bInterestRate = document.getElementById("interestRate").checked;

		var bMoneyFactor;
		bMoneyFactor = document.getElementById("moneyFactor").checked;


		if(bMoneyFactor)
		{
			value = (value * 2400).toFixed(2);
		}

		disclaimer = disclaimer.replace(disclaimer.substring(start,end),"of " + value + "% ");

		document.getElementById("cb5_10").value = disclaimer;

	}

	//Lease payments based on <> month, <> miles per year. <> down. Plus first month lease payment, refundable security deposit taxes, tags and $299 dealer processing fee. Does not include  <> disposition fee due at lease end. <> per mile over allowance. Customer responsible for excess wear and tear. On approved credit, not all customers will qualify. Must take delivery before <>. 


	function updateDisclaimerTerm(value) {

		var disclaimer;
		disclaimer = document.getElementById("cb5_10").value;

		start = disclaimer.indexOf("on");
		end = disclaimer.indexOf("month",start);

		disclaimer = disclaimer.replace(disclaimer.substring(start,end),"on " + value + " ");

		document.getElementById("cb5_10").value = disclaimer;

	}

	function updateDisclaimerRebate(value) {

		var disclaimer;
		disclaimer = document.getElementById("cb5_10").value;

		start = disclaimer.indexOf("available");
		end = disclaimer.indexOf("lease",start);

		disclaimer = disclaimer.replace(disclaimer.substring(start,end),"available $" + value + " ");

		document.getElementById("cb5_10").value = disclaimer;

	}


	function updateBuyDisclaimerTerm(value) {

		var disclaimer;
		disclaimer = document.getElementById("cb6_9").value;

		start = disclaimer.indexOf("for");
		end = disclaimer.indexOf("months",start);

		disclaimer = disclaimer.replace(disclaimer.substring(start,end),"for " + value + " ");

		document.getElementById("cb6_9").value = disclaimer;

	}

	function updateBuyDisclaimerDownPayment(value) {

		var disclaimer;
		disclaimer = document.getElementById("cb6_9").value;

		start = disclaimer.indexOf("year,");
		end = disclaimer.indexOf("down",start);

		disclaimer = disclaimer.replace(disclaimer.substring(start,end),"year, $" + value + " ");

		document.getElementById("cb6_9").value = disclaimer;

	}

	function updateBuyDisclaimerRate(value) {

		var disclaimer;
		disclaimer = document.getElementById("cb6_9").value;

		start = disclaimer.indexOf("of");
		end = disclaimer.indexOf("for",start);


		disclaimer = disclaimer.replace(disclaimer.substring(start,end),"of " + value + "% ");

		document.getElementById("cb6_9").value = disclaimer;

	}

</script>

</head>
<body link="aqua" vlink="aqua" background="imgs/home_bkgrd.gif" bgcolor="#000057">
<%


Function Chk_num(fldname,flddesc)
 Chk_num = trim(request.form(fldname))
 if chk_num = "" then chk_num = 0
 If (NOT IsNumeric(chk_num)) then
     if glblerr = 0 then
       response.write "<center><font face='arial' size=3 color='white'>"
       response.write "<p><b> The following Field(s) are Not Numeric or Not Valid Date(s) !</b><p>"
     end if

     glblerr = glblerr + 1
     response.write "<center><font face='arial' size=3 color='aqua'>"
     response.write "<br>Please correct > - " & flddesc & " <br></font>"

 End if
End Function
Function Chk_Date(fldname,flddesc)
 Chk_Date = trim(request.form(fldname))
 If chk_Date <> "" then
  If  (NOT IsDate(chk_Date)) then
     if glblerr = 0 then

       response.write "<center><font face='arial' size=3 color='white'>"
       response.write "<p><b> The following Field(s) are Not Numeric or Not Valid Date(s) ! </b><p>"
     end if

     glblerr = glblerr + 1
     response.write "<center><font face='arial' size=3 color='aqua'>"
     response.write "<br>Please correct > " & flddesc & " <br></font>"

  End if
 End If
End Function


'==========================Open FOXPRO DB
Sub OpenFOX
   Set Conn = Server.CreateObject("ADODB.connection")
   Conn.open =  Application("FDSERVER_SQL")

End Sub
'==========================BUILD SEARCH SELECT CLAUSE
Sub BldSelect

  SQLStmt = "SELECT * FROM foxprotables.dbo.Rimast "
  SQLStmt = Sqlstmt & " WHERE LOC='" & g_loc & "' "
  SQLStmt = Sqlstmt & "  AND id='" & g_id & "' "
  g_glblSQL = SQLStmt


End Sub
'================= COUNT BY MODEL AND YEAR
Sub CountModel
Dim ArrInvDays(4) 'not use it

  SQLStmt2 = "SELECT inv030,inv3160,inv6190,inv91 FROM foxprotables.dbo.Rimast "
  SQLStmt2 = Sqlstmt2 & " WHERE ltrim(rtrim(MODEL))+YEAR='" & substring(g_id,6) & "' "
   SQLStmt2 = Sqlstmt2 & "AND LOC='" & g_loc & "' "
 ' Rs.Open SQLStmt2, cmdTemp, 1,1
   Set cmdTemp = Server.CreateObject("ADODB.Command")
   Set rs2 = Server.CreateObject("ADODB.Recordset")
   cmdTemp.CommandText = SQLStmt2
   cmdTemp.CommandType = 1 'SQL statement
   Set cmdTemp.ActiveConnection = Conn
   rs2.CacheSize = 10
   rs2.Open cmdTemp,,3
   ArrInvDays(0) = rs2("inv030")
   ArrInvDays(1) = rs2("inv3160")
   ArrInvDays(2) = rs2("inv6190")
   ArrInvDays(3) =rs2("inv91")
   response.write "<font color='white'>" & SQLStmt2 & "<br></font>"
   'test display
 '  for i = 0 to ubound(ArrInvDays)
 '  response.write "<font color='white'>" &ArrInvDays(i)& "|"

 '  next
  ' response.write "</font>"
End Sub

'================= Check IF BPP Value Changed !
Sub CheckBPP
   
  SQLStmt2 = "select  * FROM foxprotables.dbo.Rimast "
  SQLStmt2 = Sqlstmt2 & " order by LOC "

   Set cmdTemp = Server.CreateObject("ADODB.Command")
   Set rs7 = Server.CreateObject("ADODB.Recordset")
   Set rs9 = Server.CreateObject("ADODB.Recordset")

   cmdTemp.CommandText = SQLStmt2
   cmdTemp.CommandType = 1 'SQL statement
   Set cmdTemp.ActiveConnection = Conn
   rs7.CacheSize = 10

   if 1=2 then
     rs7.Open cmdTemp,,3 
   '  response.write "418  <BR>DIFF="  & SQLStmt2
     do while not rs7.eof
       response.write "IPR=" &  rs7("ipr")
       IF clng(rs7("IPR")) <> clng(BPPCOST) and 1=2  then
        DIFF= clng(BPPCOST) - clng(rs7("ipr"))
        SQLStmt2 = "UPDATE foxprotables.dbo.Rimast  SET "
        SQLStmt2 = Sqlstmt2 & " ipr = " & BPPCOST & ","
        SQLStmt2 = Sqlstmt2 & " markup = markup + " & DIFF & " ,"
        SQLStmt2 = Sqlstmt2 & " markup30 = markup30 + " & DIFF & " ,"
        SQLStmt2 = Sqlstmt2 & " markup60 = markup60 + " & DIFF & " ,"
        SQLStmt2 = Sqlstmt2 & " markup90 = markup90 + " & DIFF & " ,"
        SQLStmt2 = Sqlstmt2 & " markupit = markupit + " & DIFF
        cmdTemp.CommandText = SQLStmt2
        rs9.Open cmdTemp,,3
        response.write "<BR>DIFF=" & clng(BPPCOST) - clng(rs7("ipr"))
        response.write "<BR>" & SQLStmt2
       END IF
     rs7.movenext
     loop

    
    end if

End Sub

Sub  UPD_RIMAST(DOLIST)
  '######## STEP 1.INSERT RIMAST_UPD
  '######## STEP 2.UPDATE RIMAST_UPD
  '######## STEP 3.EMAIL AND UPDATE FLAG ="Y" IN RIMAST_UPD
 'Set rs01 = Server.CreateObject("ADODB.Recordset")

    OpenFOX()  'OPEN CONN

   IF DOLIST ="INSERT_1" THEN
   COLINS = "[loc],[id],[make_code],[carline],[pkg],[year],[model],[markupit],[markup],[markup30],[markup60],[markup90],[cramt],[crstart],[crend],[crprog] "
   COLINS = COLINS & ",[fdamt],[fdmamt],[fdxamt],[fdstart],[fdend],[fdprog],[ipr],[addate],[moddate],[upuser],[lastactdt],[msrp],[spec_fin],[addl_fee],[cust_msg1]"
   COLINS = COLINS &",[cust_msg2],[cust_msg3],[cust_msg4],[inv030],[inv3160],[inv6190],[inv91],[invtot],[spec_fin_st],[spec_fin_exp],[lowest_rate],[lowest_rate24]"
   COLINS = COLINS &",[lowest_rate36],[lowest_rate48],[lowest_rate60],[lowest_rate72],[lowest_rate84] ,[lieu_rebate] "

   SQLStmtUPD = "INSERT INTO  foxprotables.dbo.RImast_UPD   "
   SQLStmtUPD = SQLStmtUPD &" ("& COLINS & ") SELECT " &  COLINS  & " FROM [FOXPROTABLES].[dbo].[Rimast] WHERE ID ='" &  session("key") &"' "
  ' RESPONSE.WRITE  "<br><br>SOI " & SQLStmtUPD  & "<br>" &  session("key")
   conn.Execute SQLStmtUPD
   END IF

   IF DOLIST ="UPDATE_2" THEN
        SQLStmt2 = "UPDATE foxprotables.dbo.RImast_UPD  SET "
        SQLStmt2 = Sqlstmt2 & " UPD_markup = B.markup -" & CINT(BPPCOST) & " ,"
        SQLStmt2 = Sqlstmt2 & " UPD_markup30 = B.markup30 -" & CINT(BPPCOST) & " ,"
        SQLStmt2 = Sqlstmt2 & " UPD_markup60 = B.markup60 -" & CINT(BPPCOST) & " ,"
        SQLStmt2 = Sqlstmt2 & " UPD_markup90 = B.markup90 -" & CINT(BPPCOST) & " ,"
	SQLStmt2 = Sqlstmt2 & " UPD_markupit = B.markupit - " & CINT(BPPCOST)& " ,"
	SQLStmt2 = Sqlstmt2 & " markup =   A.markup -" & CINT(BPPCOST) & " ,"
        SQLStmt2 = Sqlstmt2 & " markup30 = A.markup30 -" & CINT(BPPCOST) & " ,"
        SQLStmt2 = Sqlstmt2 & " markup60 = A.markup60 -" & CINT(BPPCOST) & " ,"
        SQLStmt2 = Sqlstmt2 & " markup90 = A.markup90 -" & CINT(BPPCOST) & " ,"
	SQLStmt2 = Sqlstmt2 & " markupit = A.markupit - " & CINT(BPPCOST) & " ,"
	SQLStmt2 = Sqlstmt2 & " UPD_cramt = B.cramt " & " ,"
	SQLStmt2 = Sqlstmt2 & " UPD_fdamt = B.fdamt "  & " ,"
	SQLStmt2 = Sqlstmt2 & " UPD_msrp  = B.msrp "  & " ,"
	SQLStmt2 = Sqlstmt2 & " upuser='" & session("login") &"',"
	SQLStmt2 = Sqlstmt2 & " moddate='" & date() &"'"
	SQLStmt2 = Sqlstmt2 & " FROM foxprotables.dbo.RImast_UPD A,foxprotables.dbo.RImast B  "
	SQLStmt2 = Sqlstmt2 & " WHERE A.ID ='" &  session("key") &"' AND  (FLAG IS  NULL  OR FLAG  ='') "
	SQLStmt2 = Sqlstmt2 & " AND A.ID = B.ID  "
	SQLStmt2 = Sqlstmt2 & " AND B.ID='" & session("key") &"' "

        'RESPONSE.WRITE  "<br><br>SOI " & Sqlstmt2  & "<br>" &  session("key")
WriteToTextFile(Sqlstmt2)

        conn.Execute SQLStmt2

	'#### STEP 3 SEND MAIL

     ' Location	Make	Model Number	Old Markup	New Markup	Age Range	Associate	Time & Date
       LOCATION = LEFT(session("key"),3)
       MMAKE = MID(session("key"),4,2)
       IF (LOCATION= "CDO" OR  LOCATION = "FBS"  OR  LOCATION = "LFO")  AND (MMAKE="HY" OR MMAKE="SU") THEN


          'CALL SENDMAIL(rs01("loc"),rs01("make"),rs01("model"),rs01("oldmarkup"),rs01("UPD_markup"),rs01("invtot"),rs01("upuser"))
	  CALL SENDMAIL()
         ' rs01.close
       	'#### STEP 4 change flag='Y' after send mail
        SQLStmt4 = "UPDATE foxprotables.dbo.RImast_UPD  SET "
        SQLStmt4 = Sqlstmt4 & " Flag = 'Y' "
	SQLStmt4 = Sqlstmt4 & " WHERE ID ='" &  session("key") &"' AND  (FLAG IS  NULL  OR FLAG  ='') "
        conn.Execute SQLStmt4

        END IF

	'response.end
    END IF





End Sub
'==========================The news page Display Function
Sub SubShow

  g_loc = session("loc") ' Store Loc comes from session


if g_loc = "CJE" then
  processing_fee = 499
else
  processing_fee = 299
end if



  if request.querystring("mc") <> "" then
       session("mc") = request.querystring("mc")
  end if

  session("key") = g_id
  'SELECT The record
   openFOX()

   'THIS IS WHERE WE VCHECK IF THE IPR VALUE (BPP COST) HAS CHANGED - AND ADJUST IT !!
   CheckBPP()

   BldSelect()
   SQLStmt = g_GLBLSql
   Set cmdTemp = Server.CreateObject("ADODB.Command")
   Set rs = Server.CreateObject("ADODB.Recordset")
   cmdTemp.CommandText = SQLStmt
   cmdTemp.CommandType = 1 'SQL statement
   Set cmdTemp.ActiveConnection = Conn
   rs.CacheSize = 10
   rs.Open cmdTemp,,3

 'pass value of transit from last page 7/2/08
  session("cntid") = request("cntid")  'soi need for the link 12/11/2013

if rs.EOF and rs.BOF then
'  Nothing Found
%>
                        <p><Center><font face="Arial,Arial" size="3" color="white">No Matches Found</font></p>
                        <p></center>a<%
	         Response.Write  "<center>" & session("loc") & "--" & g_glblSQL & "</center><br>"
                 Response.Write "<center><a href='pricematrixshow1.asp" & "'>Try New Search</a></center> "
  response.end
End If


makeCode = request("mc")
if makeCode = "" then
	makeCode = "XX"
end if

	' ENABLE the Leasing fields By Location, Brands and Models
	 if(InStr(g_allowedLocations,session("loc")) > 0  AND InStr(g_disallowedMakeCodes,makeCode) = 0) then
	 	g_enableLeasing = "true"
		'if(trim(rs("make_code"))  = "TO" AND InStr(g_allowedModels,trim(rs("carline"))) = 0) then
		'	g_enableLeasing = "false"
		'end if
	 End if


response.write "<br>583   " &(rs("markupit")) & BPPCOST &"==/"& clng(rs("markupit"))-BPPCOST
' LOAD THE DATABASE FIELDS TO SESSION VARS
	session("inv0_1")  = rs("inv030")  ' inv days
   	session("inv0_2") = rs("inv3160")  ' inv days
   	session("inv0_3") = rs("inv6190")  ' inv days
   	session("inv0_4")  =rs("inv91")    ' inv days
	session("inv0_5")  =rs("invtot")    ' inv days total


	 session("tm0_1") = trim(RS("year"))
	 session("tm0_2") = trim(rs("make_code"))

	 session("tm0_3") = trim(rs("model"))
	 session("tm0_4") = trim(rs("ipr"))
	 session("tm0_5") = trim(rs("carline"))


	 session("tm1_1") = clng(RS("markup"))-BPPCOST
	 session("tm1_2") = clng(rs("markup30"))-BPPCOST
	 session("tm1_3") = clng(rs("markup60"))-BPPCOST
	 session("tm1_4") = clng(rs("markup90"))-BPPCOST
	 session("tm1_5") = trim(rs("msrp"))
	 session("tm1_6") = clng(rs("markupit"))-BPPCOST ' pRE DELIVERY DISCOUNT

         session("tm2_1") = trim(rs("cramt"))
	 session("tm2_2") = trim(rs("crstart"))
	 session("tm2_3") = trim(rs("crend"))
	 session("tm2_4") = trim(rs("crprog"))


         session("tm3_1") = trim(rs("fdamt"))
	 session("tm3_2") = trim(rs("fdmamt"))
	 session("tm3_3") = trim(rs("fdxamt"))
         session("tm3_4") = trim(rs("fdstart"))
	 session("tm3_5") = trim(rs("fdend"))

         session("tm4_1") = trim(rs("spec_fin"))
	 session("tm4_2") = trim(rs("spec_fin_exp"))

	 'session("tm4_3") = trim(rs("lowest_rate"))
         session("tm4_4") = trim(rs("lieu_rebate"))
	 session("tm4_5") = trim(rs("addl_fee"))
         session("tm4_6") = trim(rs("cust_msg1"))
	 session("tm4_7") = trim(rs("cust_msg2"))
	 session("tm4_8") = trim(rs("cust_msg3"))
         session("tm4_9") = trim(rs("cust_msg4"))

         session("tm4_10") = trim(rs("lowest_rate24"))
         session("tm4_11") = trim(rs("lowest_rate36"))
         session("tm4_12") = trim(rs("lowest_rate48"))
         session("tm4_13") = trim(rs("lowest_rate60"))
         session("tm4_14") = trim(rs("lowest_rate72"))
         session("tm4_15") = trim(rs("lowest_rate84"))
         session("tm4_16") = trim(rs("spec_fin_st"))

IF (g_enableLeasing = "true") then
     session("tm5_1") = IIF(CDbl(TRIM(rs("money_factor"))) > 0, TRIM(rs("money_factor")), "0")
     session("tm5_2") = IIF(CDbl(TRIM(rs("rate_markup"))) > 0, TRIM(rs("rate_markup")), "0")
     session("tm5_3") = IIF(CDbl(TRIM(rs("lease_rebate"))) > 0, TRIM(rs("lease_rebate")), "0")
     session("tm5_4") = IIF(CDbl(TRIM(rs("residual_value"))) > 0, TRIM(rs("residual_value")), "0.00")
     session("tm5_5") = IIF(CDbl(TRIM(rs("residual_value_10k"))) > 0, TRIM(rs("residual_value_10k")), "0.00")
     session("tm5_6") = IIF(CDbl(TRIM(rs("residual_value_12k"))) > 0, TRIM(rs("residual_value_12k")), "0.00")
     session("tm5_7") = IIF(CDbl(TRIM(rs("bank_fee"))) > 0, TRIM(rs("bank_fee")), "0")
     session("tm5_8") = IIF(TRIM(rs("lieu_lease")) = "False", "", "on")
     session("tm5_9") = TRIM(rs("lease_fee"))
     session("tm5_10") = IIF(TRIM(rs("lease_discl")) <> "",TRIM(rs("lease_discl")),"Lease payments based on 36 month, <> miles per year. $0 down. Plus first month lease payment, refundable security deposit taxes, tags and $" & processing_fee & " dealer processing fee. Does not include  $0 disposition fee due at lease end. $0.00 per mile over allowance. Customer responsible for excess wear and tear. On approved credit, not all customers will qualify. Must take delivery before <>.")
     session("tm5_11") = IIF(CDbl(TRIM(rs("lease_term"))) > 0, TRIM(rs("lease_term")), "0")
     session("tm5_12") = IIF(CDbl(TRIM(rs("lease_money_down"))) > 0, TRIM(rs("lease_money_down")), "0")
     session("tm5_14") = IIF(TRIM(rs("show_payment")) = "False", "", "on")
     session("tm5_15") = IIF(trim(rs("startDateLease")) <> "1900-01-01",Right("00" & Month(trim(rs("StartDateLease"))), 2) & "/" & Right("00" & Day(trim(rs("StartDateLease"))), 2) & "/" & Year(trim(rs("StartDateLease"))),"")
     session("tm5_16") = IIF(trim(rs("ExpDateLease")) <> "1900-01-01",Right("00" & Month(trim(rs("ExpDateLease"))),2) & "/" & Right("00" & Day(trim(rs("ExpDateLease"))),2) & "/" & Year(trim(rs("ExpDateLease"))),"")
     session("tm5_17") = IIF(TRIM(rs("is_GM_lease")) = "False", "0", "1")
     session("tm5_18") = IIF(CDbl(TRIM(rs("lease_mile_charge"))) > 0, TRIM(rs("lease_mile_charge")), "0")
     session("tm5_19") = IIF(CDbl(TRIM(rs("lease_disp_fee"))) > 0, TRIM(rs("lease_disp_fee")), "0")
     session("tm5_20") = IIF(CDbl(TRIM(rs("lease_incentive"))) > 0, TRIM(rs("lease_incentive")), "0")

     session("tm6_1") = IIF(CDbl(TRIM(rs("interest_rate"))) > 0, TRIM(rs("interest_rate")), "0")
     session("tm6_2") = IIF(CDbl(TRIM(rs("down_payment"))) > 0, TRIM(rs("down_payment")), "0")
     session("tm6_3") = IIF(CDbl(TRIM(rs("term"))) > 0, TRIM(rs("term")), "0")
     session("tm6_4") = IIF(TRIM(rs("lieu_buy")) = "0", TRIM(rs("lieu_buy")), "1")
     session("tm6_5") = TRIM(rs("buypay"))
     session("tm6_6") = IIF(TRIM(rs("show_lease_payment")) = "False", "", "on")
     session("tm6_7") = IIF(trim(rs("startDateBuy")) <> "1900-01-01",Right("00" & Month(trim(rs("StartDateBuy"))), 2) & "/" & Right("00" & Day(trim(rs("StartDateBuy"))), 2) & "/" & Year(trim(rs("StartDateBuy"))),"")
     session("tm6_8") = IIF(trim(rs("ExpDateBuy")) <> "1900-01-01",Right("00" & Month(trim(rs("ExpDateBuy"))),2) & "/" & Right("00" & Day(trim(rs("ExpDateBuy"))),2) & "/" & Year(trim(rs("ExpDateBuy"))),"")
     session("tm6_9") = IIF(TRIM(rs("buyDisclaimer")) <> "",TRIM(rs("buyDisclaimer")),"Payments based on an Interest Rate of <> for <> months, with $<> down.")


End if

	 lowest()
conn.close

End Sub

' SAVE FORM VARIABLES TO SESSION (TM-) VARS
Sub SaveForm


     	 session("tm1_1") = request.form("cb1_1")
	 session("tm1_2") = request.form("cb1_1") ' changed to all use the same value
	 session("tm1_3") = request.form("cb1_1") ' changed to all use the same value
	 session("tm1_4") = request.form("cb1_1") ' changed to all use the same value
	 session("tm1_5") = request.form("cb1_5")
	 session("tm1_6") = request.form("cb1_6")

         session("tm2_1") = request.form("cb2_1")
	 session("tm2_2") = request.form("cb2_2")
	 session("tm2_3") = request.form("cb2_3")
	 session("tm2_4") = request.form("cb2_4")

         session("tm3_1") = request.form("cb3_1")
	 session("tm3_2") = request.form("cb3_2")
	 session("tm3_3") = request.form("cb3_3")
	 session("tm3_4") = request.form("cb3_4")
	 session("tm3_5") = request.form("cb3_5")

         session("tm4_1") = request.form("cb4_1")
	 session("tm4_2") = request.form("cb4_2")
	 'session("tm4_3") = request.form("cb4_3")
         session("tm4_4") = request.form("cb4_4")
	 session("tm4_5") = request.form("cb4_5")
         session("tm4_6") = request.form("cb4_6")
	 session("tm4_7") = request.form("cb4_7")
	 session("tm4_8") = request.form("cb4_8")
         session("tm4_9") = request.form("cb4_9")

         session("tm4_10") = request.form("cb4_10")
         session("tm4_11") = request.form("cb4_11")
         session("tm4_12") = request.form("cb4_12")
         session("tm4_13") = request.form("cb4_13")
         session("tm4_14") = request.form("cb4_14")
         session("tm4_15") = request.form("cb4_15")
         session("tm4_16") = request.form("cb4_16")

IF (g_enableLeasing = "true") then

	IF request.form("cb5_1") <> "" Then
     		session("tm5_1") = IIF(CDbl(TRIM(request.form("cb5_1"))) > 0, TRIM(request.form("cb5_1")), "0")
	ELSE
		session("tm5_1") = 0
	END IF

	IF request.form("cb5_2") <> "" Then
     		session("tm5_2") = IIF(CDbl(TRIM(request.form("cb5_2"))) > 0, TRIM(request.form("cb5_2")), "0")
	ELSE
		session("tm5_2") = 0
	END IF

	IF request.form("cb5_3") <> "" Then
     		session("tm5_3") = IIF(CDbl(TRIM(request.form("cb5_3"))) > 0, TRIM(request.form("cb5_3")), "0")
	ELSE
		session("tm5_3") = 0
	END IF


s15kRate = request.form("cb5_4")
s10KRate = request.form("cb5_5")
s12KRate = request.form("cb5_6")

if(s15kRate = "") then
	s15kRate = "0"
End if

if(s10kRate = "") then
	s10kRate = "0"
End if

if(s12kRate = "") then
	s12kRate = "0"
End if


     session("tm5_4") = IIF(CDbl(s15kRate) > 0, TRIM(request.form("cb5_4")), "0.00")
     session("tm5_5") = IIF(CDbl(s10kRate) > 0, TRIM(request.form("cb5_5")), "0.00")
     session("tm5_6") = IIF(CDbl(s12kRate) > 0, TRIM(request.form("cb5_6")), "0.00")

     
	IF request.form("cb5_7") <> "" Then
     		session("tm5_7") = IIF(CDbl(TRIM(request.form("cb5_7"))) > 0, TRIM(request.form("cb5_7")), "0")
	ELSE
		session("tm5_7") = 0
	END IF
     
	session("tm5_8") = TRIM(request.form("cb5_8"))
     session("tm5_9") = TRIM(request.form("cb5_9"))
     session("tm5_10") = TRIM(request.form("cb5_10"))


	IF request.form("cb5_11") <> "" Then
     		session("tm5_11") = IIF(CDbl(TRIM(request.form("cb5_11"))) > 0, TRIM(request.form("cb5_11")), "0")
	ELSE
		session("tm5_11") = 0
	END IF

	IF request.form("cb5_12") <> "" Then
     		session("tm5_12") = IIF(CDbl(TRIM(request.form("cb5_12"))) > 0, TRIM(request.form("cb5_12")), "0")
	ELSE
		session("tm5_12") = 0
	END IF
   

session("tm5_14") = trim(request.form("cb5_14"))
     session("tm5_15") = trim(request.form("cb5_15"))
     session("tm5_16") = trim(request.form("cb5_16"))
     session("tm5_18") = trim(request.form("cb5_18"))
     session("tm5_19") = trim(request.form("cb5_19"))

	IF request.form("cb5_20") <> "" Then
     		session("tm5_20") = IIF(CDbl(TRIM(request.form("cb5_20"))) > 0, TRIM(request.form("cb5_20")), "0")
	ELSE
		session("tm5_20") = 0
	END IF

rateGroup = request.form("rateGroup")
if(rateGroup = "interest") then
	session("tm5_17") = 1
else
	session("tm5_17") = 0
End if


     'session("tm6_1") = IIF(CDbl(TRIM(request.form("cb6_1"))) > 0, TRIM(request.form("cb6_1")), "0")
     'session("tm6_2") = IIF(CDbl(TRIM(request.form("cb6_2"))) > 0, TRIM(request.form("cb6_2")), "0")
     'session("tm6_3") = IIF(CDbl(TRIM(request.form("cb6_3"))) > 0, TRIM(request.form("cb6_3")), "0")
     'session("tm6_4") = IIF(TRIM(request.form("cb6_4")) = "0", TRIM(request.form("cb6_4")), "1")
     'session("tm6_5") = TRIM(request.form("cb6_5"))
     'session("tm6_6") = TRIM(request.form("cb6_6"))
     'session("tm6_7") = TRIM(request.form("cb6_7"))
     'session("tm6_8") = TRIM(request.form("cb6_8"))
     'session("tm6_9") = TRIM(request.form("cb6_9"))

End if

WriteToTextFile("Form Saved")

End Sub
' ==============  SAVE Data!!
Sub SaveData


makeCode = session("tm0_2")
if makeCode = "" then
	makeCode = "XX"
end if

WriteToTextFile("MakeCode: " & makeCode)

	 if(InStr(g_allowedLocations,session("loc")) > 0  AND InStr(g_disallowedMakeCodes,makeCode) = 0) then		
	 	g_enableLeasing = "true"

		'if(session("tm0_2") = "TO" AND InStr(g_allowedModels,session("tm0_5")) = 0) then
		'	g_enableLeasing = "false"
		'end if
	 End if

WriteToTextFile("Leasing enabled: " & g_enableLeasing)

        session("tm1_1")=chk_num("cb1_1","Base Red Tag Markup (00-30 Days)") ' changed to all use the same value
	session("tm1_2")=chk_num("cb1_1","Base Red Tag Markup (31-30 Days)") ' changed to all use the same value
	session("tm1_3")=chk_num("cb1_1","Base Red Tag Markup (61-30 Days)") ' changed to all use the same value
        session("tm1_4")=chk_num("cb1_1","Base Red Tag Markup (91-30 Days)") ' changed to all use the same value
	session("tm1_5")=TRIM(request.form("cb1_5"))
	session("tm1_6")=chk_num("cb1_6","On Order Discount")

	session("tm2_1")=chk_num("cb2_1","Consumer Rebate (Rebate Amt.)")
        session("tm2_2")=chk_date("cb2_2","Consumer Rebate (Start Date)")
	session("tm2_3")=chk_date("cb2_3","Consumer Rebate (End Date)")
	session("tm2_4")= TRIM(request.form("cb2_4")) & " "

	session("tm3_1")=chk_num("cb3_1","Dealer Incentive (Amt Applied to Price)")
	session("tm3_2")=chk_num("cb3_2","Dealer Incentive (Minimum)")
	session("tm3_3")=chk_num("cb3_3","Dealer Incentive (Maximum)")
	session("tm3_4")=chk_date("cb3_4","Dealer Incentive (Start Date)")
	session("tm3_5")=chk_date("cb3_5","Dealer Incentive (End Date)")
        session("tm4_1") = TRIM(request.form("cb4_1"))
	if ucase(trim(session("tm4_1"))) = "Y" then
	  session("tm4_2") = chk_date("cb4_2","Special Financing(Expiration Date)")
  	  'session("tm4_3") = TRIM(request.form("cb4_3"))
          session("tm4_4") = TRIM(request.form("cb4_4"))
	  session("tm4_5") = TRIM(request.form("cb4_5"))
          session("tm4_6") = TRIM(request.form("cb4_6"))
	  session("tm4_7") = TRIM(request.form("cb4_7"))
	  session("tm4_8") = TRIM(request.form("cb4_8"))
          session("tm4_9") = TRIM(request.form("cb4_9"))

          session("tm4_10") = TRIM(request.form("cb4_10"))
          session("tm4_11") = TRIM(request.form("cb4_11"))
          session("tm4_12") = TRIM(request.form("cb4_12"))
          session("tm4_13") = TRIM(request.form("cb4_13"))
          session("tm4_14") = TRIM(request.form("cb4_14"))
          session("tm4_15") = TRIM(request.form("cb4_15"))
          session("tm4_16") = TRIM(request.form("cb4_16"))
	else
	  session("tm4_2") = ""
	  'session("tm4_3") = ""
          session("tm4_4") = ""
	  session("tm4_5") = ""
          session("tm4_6") = ""
	  session("tm4_7") = ""
	  session("tm4_8") = ""
          session("tm4_9") = ""

          session("tm4_10") = ""
          session("tm4_11") = ""
          session("tm4_12") = ""
          session("tm4_13") = ""
          session("tm4_14") = ""
          session("tm4_15") = ""
          session("tm4_16") = ""
	end if

IF (g_enableLeasing = "true") then

ResidualValue15K = request.form("cb5_4")
ResidualValue10K = request.form("cb5_5")
ResidualValue12K = request.form("cb5_6")

ResidualValue15K = IIF(ResidualValue15K = "", "0.00", ResidualValue15K)
ResidualValue10K = IIF(ResidualValue10K = "", "0.00", ResidualValue10K) 
ResidualValue12K = IIF(ResidualValue12K = "", "0.00", ResidualValue12K)

	IF request.form("cb5_1") <> "" Then
        	session("tm5_1") = IIF(TRIM(request.form("cb5_1")) = "0", "0", TRIM(request.form("cb5_1")))
	ELSE
		session("tm5_1 ") = 0
	END IF


	IF request.form("cb5_2") <> "" Then
     		session("tm5_2") = IIF(CDbl(TRIM(request.form("cb5_2"))) > 0, TRIM(request.form("cb5_2")), "0")
	ELSE
		session("tm5_2") = 0
	END IF

	IF request.form("cb5_3") <> "" Then
     		session("tm5_3") = IIF(CDbl(TRIM(request.form("cb5_3"))) > 0, TRIM(request.form("cb5_3")), "0")
	ELSE
		session("tm5_3") = 0
	END IF

	IF request.form("cb5_4") <> "" Then
     		session("tm5_4") = IIF(CDbl(TRIM(ResidualValue15K)) > 0, TRIM(ResidualValue15K), "0.00")
	ELSE
		session("tm5_4") = 0
	END IF

	IF request.form("cb5_5") <> "" Then
     		session("tm5_5") = IIF(CDbl(TRIM(ResidualValue10K)) > 0, TRIM(ResidualValue10K), "0.00")
	ELSE
		session("tm5_5") = 0
	END IF

	IF request.form("cb5_6") <> "" Then
     		session("tm5_6") = IIF(CDbl(TRIM(ResidualValue12K)) > 0, TRIM(ResidualValue12K), "0.00")
	ELSE
		session("tm5_6") = 0
	END IF

	IF request.form("cb5_7") <> "" Then
     		session("tm5_7") = IIF(CDbl(TRIM(request.form("cb5_7"))) > 0, TRIM(request.form("cb5_7")), "0")
	ELSE
		session("tm5_7") = 0
	END IF
    
        session("tm5_8") = TRIM(request.form("cb5_8"))

	IF request.form("cb5_9") <> "" Then
     		session("tm5_9") = IIF(CDbl(TRIM(request.form("cb5_9"))) > 0, TRIM(request.form("cb5_9")), "0")
	ELSE
		session("tm5_9") = 0
	END IF


        session("tm5_10") = TRIM(request.form("cb5_10"))

	IF request.form("cb5_11") <> "" Then
     		session("tm5_11") = IIF(CDbl(TRIM(request.form("cb5_11"))) > 0, TRIM(request.form("cb5_11")), "0")
	ELSE
		session("tm5_11") = 0
	END IF

	IF request.form("cb5_12") <> "" Then
     		session("tm5_12") = IIF(CDbl(TRIM(request.form("cb5_12"))) > 0, TRIM(request.form("cb5_12")), "0")
	ELSE
		session("tm5_12") = 0
	END IF

	session("tm5_14") = TRIM(request.form("cb5_14"))
    	session("tm5_15") = chk_date("cb5_15","Lease Payment (Start Date)") 'trim(request.form("cb5_15"))
     	session("tm5_16") = chk_date("cb5_16","Lease Payment (End Date)") 'trim(request.form("cb5_16"))

rateGroup = request.form("rateGroup")
if(rateGroup = "interest") then
	session("tm5_17") = 1
else
	session("tm5_17") = 0
End if


	IF request.form("cb5_18") <> "" Then
     		session("tm5_18") = IIF(CDbl(TRIM(request.form("cb5_18"))) > 0, TRIM(request.form("cb5_18")), "0")
	ELSE
		session("tm5_18") = 0
	END IF

	IF request.form("cb5_19") <> "" Then
     		session("tm5_19") = IIF(CDbl(TRIM(request.form("cb5_19"))) > 0, TRIM(request.form("cb5_19")), "0")
	ELSE
		session("tm5_19") = 0
	END IF

	IF request.form("cb5_20") <> "" Then
     		session("tm5_20") = IIF(CDbl(TRIM(request.form("cb5_20"))) > 0, TRIM(request.form("cb5_20")), "0")
	ELSE
		session("tm5_20") = 0
	END IF

        'session("tm6_1") = IIF(CDbl(TRIM(request.form("cb6_1"))) > 0, TRIM(request.form("cb6_1")), "0")
	'    session("tm6_2") = IIF(CDbl(TRIM(request.form("cb6_2"))) > 0, TRIM(request.form("cb6_2")), "0")
  	'    session("tm6_3") = IIF(CDbl(TRIM(request.form("cb6_3"))) > 0, TRIM(request.form("cb6_3")), "0")
  	'    session("tm6_4") = IIF(TRIM(request.form("cb6_4")) = "0", TRIM(request.form("cb6_4")), "1")
  	'    session("tm6_5") = TRIM(request.form("cb6_5"))
	'session("tm6_6") = TRIM(request.form("cb6_6"))
     'session("tm6_7") = chk_date("cb6_7","Payment (Start Date)") 'TRIM(request.form("cb6_7"))
     'session("tm6_8") = chk_date("cb6_8","Payment (End Date)")'TRIM(request.form("cb6_8"))
	'session("tm6_9") = TRIM(request.form("cb6_9"))	
End If

IF (g_enableLeasing = "true") then
'OR (CDbl(TRIM(request.form("cb6_1"))) > -1)

	IF request.form("cb5_1") <> "" Then
     		hasRate = CDbl(TRIM(request.form("cb5_1")))
	ELSE
		hasRate = 0
	END IF
WriteToTextFile("Rate Value: " & hasRate)

    IF ((hasRate > -1)  ) THEN    

	WriteToTextFile("Calling Matrix Lease Calculations for " & session("key"))
 	  Set HttpReq = Server.CreateObject("MSXML2.ServerXMLHTTP")
	  HttpReq.open "GET", "http://10.254.162.190/services/matrix/matrixleasecalc/?id=" & session("key"), False
	  HttpReq.send
    END IF
End If

	If glblerr = 0 Then
	 g_loc = session("loc") ' Store Loc comes from session
         g_id = session("key")
	 openFOX()  'connection foxpro
	 moddate =(Month(Date) & "/" & Day(Date) & "/" & Year(Date))
         SQL1 = "UPDATE foxprotables.dbo.Rimast SET "
	 SQL1 = SQL1 + "upuser = '" & session("login") & "',"
	 SQL1 = SQL1 + "moddate = '" & moddate & "',"
	 SQL1 = SQL1 + "markup = " & clng(session("tm1_1"))+clng(session("tm0_4")) & ","
	 SQL1 = SQL1 + "markup30 = " & clng(session("tm1_2"))+clng(session("tm0_4")) & ","
	 SQL1 = SQL1 + "markup60 = " & clng(session("tm1_3"))+clng(session("tm0_4")) & ","
	 SQL1 = SQL1 + "markup90 = " & clng(session("tm1_4"))+clng(session("tm0_4")) & ","
	 SQL1 = SQL1 + "msrp = '" & session("tm1_5") & "', "
	 SQL1 = SQL1 + "markupIT = " & clng(session("tm1_6"))+clng(session("tm0_4")) & ", "

	 SQL1 = SQL1 + "CRAMT = " & session("tm2_1") & ", "
	 SQL1 = SQL1 + "CRSTART = '" & session("tm2_2") & "', "
	 SQL1 = SQL1 + "CREND = '" & session("tm2_3") & "', "
	 SQL1 = SQL1 + "CRPROG = '" & session("tm2_4") & "', "

	 SQL1 = SQL1 + "FDAMT = " & session("tm3_1") & ", "
	 SQL1 = SQL1 + "FDMAMT = " & session("tm3_2") & ", "
  	 SQL1 = SQL1 + "FDXAMT = " & session("tm3_3") & ", "
	 SQL1 = SQL1 + "FDSTART = '" & session("tm3_4") & "', "
	 SQL1 = SQL1 + "FDEND = '" & session("tm3_5") & "', "

	 SQL1 = SQL1 + "SPEC_FIN = '" & session("tm4_1") & "', "
	 SQL1 = SQL1 + "SPEC_FIN_ST = '" & session("tm4_16") & "', "
	 SQL1 = SQL1 + "SPEC_FIN_EXP = '" & session("tm4_2") & "', "
  	 SQL1 = SQL1 + "LOWEST_RATE = '" & session("tm4_3") & "', "

	 SQL1 = SQL1 + "LOWEST_RATE24 = '" & session("tm4_10") & "', "
	 SQL1 = SQL1 + "LOWEST_RATE36 = '" & session("tm4_11") & "', "
	 SQL1 = SQL1 + "LOWEST_RATE48 = '" & session("tm4_12") & "', "
	 SQL1 = SQL1 + "LOWEST_RATE60 = '" & session("tm4_13") & "', "
	 SQL1 = SQL1 + "LOWEST_RATE72 = '" & session("tm4_14") & "', "
	 SQL1 = SQL1 + "LOWEST_RATE84 = '" & session("tm4_15") & "', "


	 SQL1 = SQL1 + "LIEU_REBATE = '" & session("tm4_4") & "', "
	 SQL1 = SQL1 + "ADDL_FEE = '" & session("tm4_5") & "', "
	 SQL1 = SQL1 + "CUST_MSG1 = '" & session("tm4_6") & "', "
  	 SQL1 = SQL1 + "CUST_MSG2 = '" & session("tm4_7") & "', "
	 SQL1 = SQL1 + "CUST_MSG3 = '" & session("tm4_8") & "', "
	 SQL1 = SQL1 + "CUST_MSG4 = '" & session("tm4_9") & "' "


IF (g_enableLeasing = "true") then

	 SQL1 = SQL1 + ", MONEY_FACTOR = '" & session("tm5_1") & "', "
     SQL1 = SQL1 + "RATE_MARKUP = '" & session("tm5_2") & "', "
     SQL1 = SQL1 + "LEASE_REBATE = '" & session("tm5_3") & "', "
     SQL1 = SQL1 + "RESIDUAL_VALUE = '" & session("tm5_4") & "', "
     SQL1 = SQL1 + "RESIDUAL_VALUE_10K = '" & session("tm5_5") & "', "
     SQL1 = SQL1 + "RESIDUAL_VALUE_12K = '" & session("tm5_6") & "', "
     SQL1 = SQL1 + "BANK_FEE = '" & session("tm5_7") & "', "
     'SQL1 = SQL1 + "LIEU_LEASE = '" & IIF(session("tm5_8") = "on", "1", "0") & "', "
	SQL1 = SQL1 + "LIEU_LEASE = '1', "

if  g_loc = "CJE" then
     SQL1 = SQL1 + "LEASE_FEE = '" & session("tm5_9") & "', "
end if

     SQL1 = SQL1 + "lease_discl = '" & session("tm5_10") & "', "
     'SQL1 = SQL1 + "buyDisclaimer = '" & session("tm6_9") & "', "
     'SQL1 = SQL1 + "INTEREST_RATE = '" & session("tm6_1") & "', "
     'SQL1 = SQL1 + "DOWN_PAYMENT = '" & session("tm6_2") & "', "
     'SQL1 = SQL1 + "TERM = '" & session("tm6_3") & "', "
     'SQL1 = SQL1 + "LIEU_BUY = '" & session("tm6_4") & "', "
     SQL1 = SQL1 + "lease_term = '" & session("tm5_11") & "', "
     SQL1 = SQL1 + "lease_money_down = '" & session("tm5_12") & "', "

     SQL1 = SQL1 + "startDateLease = '" & session("tm5_15") & "', "
     SQL1 = SQL1 + "ExpDateLease = '" & session("tm5_16") & "', "
     'SQL1 = SQL1 + "startDateBuy = '" & session("tm6_7") & "', "
     'SQL1 = SQL1 + "ExpDateBuy = '" & session("tm6_8") & "', "
     SQL1 = SQL1 + "show_lease_payment = '" & IIF(session("tm5_14") = "on", "1", "0") & "', "
     'SQL1 = SQL1 + "show_payment = '" & IIF(session("tm6_6") = "on", "1", "0") & "', "
	SQL1 = SQL1 + "is_GM_lease = '" & session("tm5_17") & "', "		
	SQL1 = SQL1 + "lease_mile_charge = '" & session("tm5_18") & "', "		
	SQL1 = SQL1 + "lease_disp_fee = '" & session("tm5_19") & "' ,"		
     SQL1 = SQL1 + "lease_incentive = '" & session("tm5_20") & "' "
'Response.Write(session("tm5_17"))
'Response.Write("Lieu = " & session("tm5_8"))

End if

	 SQL1 = Sql1 & " WHERE LOC='" & g_loc & "' "
         SQL1 = Sql1 & " AND id='" & g_id & "' "


	  ' Create a recordset
	  Set Rs = Server.CreateObject("ADODB.RecordSet")
          SQLStmt = SQL1
          Set cmdTemp = Server.CreateObject("ADODB.Command")
          Set rs = Server.CreateObject("ADODB.Recordset")
          cmdTemp.CommandText = SQLStmt
          cmdTemp.CommandType = 1 'SQL statement
          Set cmdTemp.ActiveConnection = Conn
	  response.write "----" & Sql1
WriteToTextFile(Sql1)
          rs.Open cmdTemp

	 'Rs.Close
	 Set Rs = Nothing
	 conn.close

	ENd if
End Sub

'==========================START MAINLINE CODE
Randomize()
intRangeSize = 999999999 - 0 + 1
sngRandomValue = intRangeSize * Rnd()
Rand = Int(sngRandomValue)

WriteToTextFile("starting main code")

g_id = trim(request("id"))
fx = request("fx")

if request.form("submit") <> "" then
  fx = request.form("submit")
	'Call SaveIncentives()
  Call UPD_RIMAST("INSERT_1")	'12/11/2013 UPD RIMAST INSERT BEFORE SAVE
end if

WriteToTextFile("fx value: " & fx)
'
if fx = "edit" then ' We came from List - save Key
   SubShow()


else
    'response.write "<BR>" & fx

WriteToTextFile("Saving Matrix values")

makeCode = session("tm0_2")
if makeCode = "" then
	makeCode = "XX"
end if

	 if(InStr(g_allowedLocations,session("loc")) > 0  AND InStr(g_disallowedMakeCodes,makeCode) = 0) then		
	 	g_enableLeasing = "true"

		'if(session("tm0_2") = "TO" AND InStr(g_allowedModels,session("tm0_5")) = 0) then
		'	g_enableLeasing = "false"
		'end if
	 End if

WriteToTextFile(g_enableLeasing)

  select case fx
  case "RECALCULATE"
         SaveForm()
	     'response.write "<BR>RECALC"
  case "XCLEAR2"
         session("tm2_1") = ""
	 session("tm2_2") = ""
	 session("tm2_3") = ""
	 session("tm2_4") = ""

  case "XCLEAR3"
         session("tm3_1") = ""
	 session("tm3_2") = ""
	 session("tm3_3") = ""
	 session("tm3_4") = ""
	 session("tm3_5") = ""

  case "XCLEAR4"
         session("tm4_1") = ""
	 session("tm4_2") = ""
	 session("tm4_3") = ""
	 session("tm4_4") = ""
	 session("tm4_5") = ""
	 session("tm4_6") = ""
	 session("tm4_7") = ""
	 session("tm4_8") = ""
	 session("tm4_9") = ""

	 session("tm4_10") = ""
	 session("tm4_11") = ""
	 session("tm4_12") = ""
	 session("tm4_13") = ""
	 session("tm4_14") = ""
	 session("tm4_15") = ""
	 session("tm4_16") = ""

CASE "XCLEAR5"
     session("tm5_1") = 0
  	 session("tm5_2") = 0
     session("tm5_3") = 0
     session("tm5_4") = 0
     session("tm5_5") = 0
     session("tm5_6") = 0
     session("tm5_7") = 0
     session("tm5_8") = 0
     session("tm5_9") = 0
     session("tm5_10") = 0
     session("tm5_11") = 0
     session("tm5_12") = 0
     session("tm5_15") = ""
     session("tm5_16") = ""
     session("tm5_18") = 0
     session("tm5_19") = 0
     session("tm5_20") = 0

  CASE "XCLEAR6"
     session("tm6_1") = ""
  	 session("tm6_2") = ""
     session("tm6_3") = ""
     session("tm6_4") = ""
	 session("tm6_5") = ""
	 session("tm6_7") = ""
	 session("tm6_8") = ""
	 session("tm6_9") = ""


  case "CLEAR1"
         SaveForm()
         session("cb1_1") = ""
	 'session("cb1_2") = ""
	 'session("cb1_3") = ""
	 'session("cb1_4") = ""
	 session("cb1_5") = ""
	 session("cb1_6") = ""

  case "CLEAR2"
         SaveForm()
         session("cb2_1") = ""
	 session("cb2_2") = ""
	 session("cb2_3") = ""
	 session("cb2_4") = ""
	 session("cb2_5") = ""
         session("tm2_1") = ""
	 session("tm2_2") = ""
	 session("tm2_3") = ""
	 session("tm2_4") = ""

  case "CLEAR3"
         SaveForm()
         session("cb3_1") = ""
	 session("cb3_2") = ""
	 session("cb3_3") = ""
	 session("cb3_4") = ""
	 session("cb3_5") = ""
         session("tm3_1") = ""
	 session("tm3_2") = ""
	 session("tm3_3") = ""
	 session("tm3_4") = ""
	 session("tm3_5") = ""

  case "CLEAR4"
         SaveForm()
         session("cb4_1") = ""
	 session("cb4_2") = ""
	 session("cb4_3") = ""
	 session("cb4_4") = ""
	 session("cb4_5") = ""
	 session("cb4_6") = ""
	 session("cb4_7") = ""
	 session("cb4_8") = ""
	 session("cb4_9") = ""

	 session("cb4_10") = ""
	 session("cb4_11") = ""
	 session("cb4_12") = ""
	 session("cb4_13") = ""
	 session("cb4_14") = ""
	 session("cb4_15") = ""
	 session("cb4_16") = ""
         session("tm4_1") = ""
	 session("tm4_2") = ""
	 session("tm4_3") = ""
	 session("tm4_4") = ""
	 session("tm4_5") = ""
	 session("tm4_6") = ""
	 session("tm4_7") = ""
	 session("tm4_8") = ""
	 session("tm4_9") = ""

	 session("tm4_10") = ""
	 session("tm4_11") = ""
	 session("tm4_12") = ""
	 session("tm4_13") = ""
	 session("tm4_14") = ""
	 session("tm4_15") = ""
  CASE "CLEAR5"
     session("cb5_1") = ""
  	 session("cb5_2") = ""
     session("cb5_3") = ""
     session("cb5_4") = ""
     session("cb5_5") = ""
     session("cb5_6") = ""
     session("cb5_7") = ""
     session("cb5_8") = ""
     session("cb5_9") = ""
     session("cb5_10") = ""
     session("cb5_11") = ""
     session("cb5_12") = ""
     session("cb5_15") = ""
     session("cb5_16") = ""
     session("cb5_18") = ""
     session("cb5_19") = ""
     session("cb5_20") = ""
     session("tm5_1") = ""
  	session("tm5_2") = ""
     session("tm5_3") = ""
     session("tm5_4") = 0
     session("tm5_5") = 0
     session("tm5_6") = 0
     session("tm5_7") = ""
     session("tm5_8") = ""
     session("tm5_9") = ""
     session("tm5_10") = ""
     session("tm5_11") = ""
     session("tm5_12") = ""
     session("tm5_15") = ""
     session("tm5_16") = ""
     session("tm5_18") = ""
     session("tm5_19") = ""
     session("tm5_20") = ""

  CASE "CLEAR6"
     session("cb6_1") = ""
  	 session("cb6_2") = ""
     session("cb6_3") = ""
     session("cb6_4") = ""
     session("cb6_5") = ""
     session("cb6_7") = ""
     session("cb6_8") = ""
     session("cb6_9") = ""

  case "COPY1"
         SaveForm()
         session("cb1_1") = request.form("cb1_1")
	 'session("cb1_2") = request.form("cb1_2")
	 'session("cb1_3") = request.form("cb1_3")
	 'session("cb1_4") = request.form("cb1_4")
	 session("cb1_5") = request.form("cb1_5")
	 session("cb1_6") = request.form("cb1_6")
  case "COPY2"
         SaveForm()
         session("cb2_1") = request.form("cb2_1")
	 session("cb2_2") = request.form("cb2_2")
	 session("cb2_3") = request.form("cb2_3")
	 session("cb2_4") = request.form("cb2_4")
	 session("cb2_5") = request.form("cb2_5")
  case "COPY3"
         SaveForm()
         session("cb3_1") = request.form("cb3_1")
	 session("cb3_2") = request.form("cb3_2")
	 session("cb3_3") = request.form("cb3_3")
	 session("cb3_4") = request.form("cb3_4")
	 session("cb3_5") = request.form("cb3_5")

  case "COPY4"
         SaveForm()
         session("cb4_1") = request.form("cb4_1")
	 session("cb4_2") = request.form("cb4_2")
	 session("cb4_3") = request.form("cb4_3")
	 session("cb4_4") = request.form("cb4_4")
	 session("cb4_5") = request.form("cb4_5")
	 session("cb4_6") = request.form("cb4_6")
	 session("cb4_7") = request.form("cb4_7")
	 session("cb4_8") = request.form("cb4_8")
	 session("cb4_9") = request.form("cb4_9")

	 session("cb4_10") = request.form("cb4_10")
	 session("cb4_11") = request.form("cb4_11")
	 session("cb4_12") = request.form("cb4_12")
	 session("cb4_13") = request.form("cb4_13")
	 session("cb4_14") = request.form("cb4_14")
	 session("cb4_15") = request.form("cb4_15")
	 session("cb4_16") = request.form("cb4_16")

  case "COPY5"
     SaveForm()
     session("cb5_1") = request.form("cb5_1")
	 session("cb5_2") = request.form("cb5_2")
	 session("cb5_3") = request.form("cb5_3")
	 session("cb5_4") = request.form("cb5_4")
	 session("cb5_5") = request.form("cb5_5")
	 session("cb5_6") = request.form("cb5_6")
	 session("cb5_7") = request.form("cb5_7")
	 session("cb5_8") = request.form("cb5_8")
	 session("cb5_9") = request.form("cb5_9")
     session("cb5_10") = request.form("cb5_10")
     session("cb5_11") = request.form("cb5_11")
     session("cb5_12") = request.form("cb5_12")
     session("cb5_15") = request.form("cb5_15")
     session("cb5_16") = request.form("cb5_16")
     session("cb5_18") = request.form("cb5_18")
     session("cb5_19") = request.form("cb5_19")
     session("cb5_20") = request.form("cb5_20")

  case "COPY6"
     SaveForm()
     session("cb6_1") = request.form("cb6_1")
	 session("cb6_2") = request.form("cb6_2")
	 session("cb6_3") = request.form("cb6_3")
	 session("cb6_4") = request.form("cb6_4")
	 session("cb6_5") = request.form("cb6_5")
	 session("cb6_7") = request.form("cb6_7")
	 session("cb6_8") = request.form("cb6_8")
	 session("cb6_9") = request.form("cb6_9")
  case "PASTE1"
         SaveForm()
         session("tm1_1") = session("cb1_1") ' changed to all use the same value
	 session("tm1_2") = session("cb1_1") ' changed to all use the same value
	 session("tm1_3") = session("cb1_1") ' changed to all use the same value
	 session("tm1_4") = session("cb1_1") ' changed to all use the same value
	 session("tm1_5") = session("cb1_5")
	 session("tm1_6") = session("cb1_6")

  case "PASTE2"
         SaveForm()
         session("tm2_1") = session("cb2_1")
	 session("tm2_2") = session("cb2_2")
	 session("tm2_3") = session("cb2_3")
	 session("tm2_4") = session("cb2_4")

  case "PASTE3"
         SaveForm()
         session("tm3_1") = session("cb3_1")
	 session("tm3_2") = session("cb3_2")
	 session("tm3_3") = session("cb3_3")
	 session("tm3_4") = session("cb3_4")
	 session("tm3_5") = session("cb3_5")

  case "PASTE4"
         SaveForm()
         session("tm4_1") = session("cb4_1")
	 session("tm4_2") = session("cb4_2")
	 session("tm4_3") = session("cb4_3")
	 session("tm4_4") = session("cb4_4")
	 session("tm4_5") = session("cb4_5")
	 session("tm4_6") = session("cb4_6")
	 session("tm4_7") = session("cb4_7")
	 session("tm4_8") = session("cb4_8")
	 session("tm4_9") = session("cb4_9")

	 session("tm4_10") = session("cb4_10")
	 session("tm4_11") = session("cb4_11")
	 session("tm4_12") = session("cb4_12")
	 session("tm4_13") = session("cb4_13")
	 session("tm4_14") = session("cb4_14")
	 session("tm4_15") = session("cb4_15")
	 session("tm4_16") = session("cb4_16")

  case "PASTE5"
     SaveForm()
     session("tm5_1") = session("cb5_1")
	 session("tm5_2") = session("cb5_2")
	 session("tm5_3") = session("cb5_3")
	 session("tm5_4") = session("cb5_4")
	 session("tm5_5") = session("cb5_5")
	 session("tm5_6") = session("cb5_6")
	 session("tm5_7") = session("cb5_7")
	 session("tm5_8") = session("cb5_8")
	 session("tm5_9") = session("cb5_9")
     session("tm5_10") = session("cb5_10")
     session("tm5_11") = session("cb5_11")
     session("tm5_12") = session("cb5_12")
     session("tm5_15") = session("cb5_15")
     session("tm5_16") = session("cb5_16")
     session("tm5_18") = session("cb5_18")
     session("tm5_19") = session("cb5_19")
     session("tm5_20") = session("cb5_20")

  case "PASTE6"
     SaveForm()
     session("tm6_1") = session("cb6_1")
	 session("tm6_2") = session("cb6_2")
	 session("tm6_3") = session("cb6_3")
	 session("tm6_4") = session("cb6_4")
     session("tm6_5") = session("cb6_5")
     session("tm6_7") = session("cb6_7")
     session("tm6_8") = session("cb6_8")
     session("tm6_9") = session("cb6_9")
  case ELSE
	WriteToTextFile("Calling SaveForm...")
       SaveForm()
	WriteToTextFile("Calling SaveData...")
       SaveData()
	WriteToTextFile("Calling UPD_RIMAST(UPDATE_2)...")
       UPD_RIMAST("UPDATE_2")

       if glblerr > 0 then
       %>

<p align="center"><font color="white" face="Arial" size="3">Click your Browsers
 </font><a href="pricematrixedit1.asp?fx=edit&id=<%=session("key")%>"><font color="white" face="Arial" size="3">&quot;</font><font face="Arial" size="3" color="white">
</font><font face="Arial" size="3" color="#FF6633">BACK</font><font face="Arial" size="3" color="white">&quot;
</font></a><font face="Arial" size="3" color="white">&nbsp;Button to Correct the Fields !</font><font face="Arial" size="3">       </font><%
         response.end
       else
        Response.Clear
        Response.Redirect "pricematrixshow1.asp?lxid=" & g_id & "&rand=" & rand & "#" & g_id
	'response.end
       end if
End Select
End if
msrp1 = "checked"
msrp2 = ""
if session("tm1_5")="I" then
 msrp1 = ""
 msrp2 = "checked"
end if
'===========================
'Yes/No
'===========================
yn41a = ""
yn41b = "checked"
if session("tm4_1") = "Y" Then
 yn41a = "checked"
 yn41b = ""
End If

yn44a = ""
yn44b = "checked"
if session("tm4_4") = "Y" Then
 yn44a = "checked"
 yn44b = ""
End If

yn45a = ""
yn45b = "checked"
if session("tm4_5") = "Y" Then
 yn45a = "checked"
 yn45b = ""
End If
'===========================
'

selected10K = ""
selected12K = ""
selected15K = ""
showLease = ""
showPayment = ""
isGM = ""
notGM = ""
LieuLease = ""

if session("tm5_4") > 0 Then
	selected15K = "checked"
End if
if session("tm5_5") > 0 Then
	selected10K = "checked"
End if
if session("tm5_6") > 0 Then
	selected12K = "checked"
End if

If session("tm6_6") = "on" then
	showLease = "checked"
End if

If session("tm5_14") = "on" then
	showPayment = "checked"
End if

if session("tm5_17") = 1 then
	isGM = "checked"
else
	notGM = "checked"
End if

If session("tm5_8") = "on" then
	LieuLease = "checked"
End If

%>
<form name="form2" method="post" action="pricematrixedit1.asp">
<table border="1" bgcolor="#333333" align="center" width="976">
    <tr>
        <td width="248" align="center" valign="middle" bgcolor="aqua">
<p align="center"><a href="../default.asp?rand=<%=Int(99999999 * Rnd())%>"><img src="../images/bluebuthome.gif" width="135" height="28" border="0"></a> </p>
        </td>
        <td width="412" align="center" valign="middle" bgcolor="aqua">
<p align="center">
                <font face="Arial" size="2">&nbsp;</font><font face="Arial" size="2" color="white"><b><span style="background-color:red;">&nbsp;&nbsp;&nbsp;&nbsp;EDIT
                / UPDATE PRICE MATRIX <br></span></b></font><font face="Arial" size="1" color="#000057"><b>&nbsp;</b></font><a href='pricematrixshow1.asp?lxid=<%=g_id%>&rand=<%=rand%>#<%=g_id%>'><font face="Arial" size="1" color="#000057">BACK
                TO LIST</font></a></p>
        </td>
        <td width="294" align="center" valign="middle" bgcolor="aqua">
<p align="center"><a href="pricemathelp.asp"><b><font face="Arial" color="blue"><img src="../images/bluebuthelp.gif" width="135" height="28" border="0"></font></b></a></p>
        </td>
    </tr>
    <tr>
        <td width="966" align="center" valign="middle" bgcolor="aqua" colspan="3">
               <font face="Arial" size="2" color="black">Year:</font> <font face="Arial" size="2" color="blue"><% =session("tm0_1")%> &nbsp;&nbsp;</font><font face="Arial" size="2" color="black">Make:</font><font face="Arial" size="2" color="white">:</font><font face="Arial" size="2" color="bLUE">&nbsp; </font><font face="Arial" size="2" color="blue"><% =session("tm0_2")%> &nbsp;&nbsp;</font><font face="Arial" size="2" color="black">Model:</font><font face="Arial" size="2" color="bLUE">&nbsp; </font><font face="Arial" size="2" color="blue"><% =session("tm0_3")%> &nbsp;&nbsp;</font><font face="Arial" size="2" color="black">Carline:</font><font face="Arial" size="2" color="bLUE">&nbsp; </font><font face="Arial" size="2" color="blue"><% =session("tm0_5")%></font>
        </td>
    </tr>
    <tr>
        <td width="966" colspan="3">
            <table border="1" width="964">
                <tr>
                    <td width="453" align="center" valign="top">

		    <table border="1" width="448" bgcolor="aqua" cellspacing="0" bordercolordark="white" bordercolorlight="black">
                            <tr>
                                <td align="left" valign="top" colspan="4" bgcolor="#669999" width="442">
                                    <p align="center"><font face="Arial" size="2" color="white">&nbsp;Internet
                                        Price Calculation</font></p>
                                </td>
                            </tr>
			     <%
			       if (request("cntid")) = "" then
			           Pre_cntid = 0
			       else
  		       	          Pre_cntid = (request("cntid"))
			       end if

			       inTransit14 =Pre_cntid - ( clng(session("inv0_1"))+clng(session("inv0_2"))+clng(session("inv0_3"))+clng(session("inv0_4")))

			       %>
                            <tr>
                                <td width="82" align="center" valign="middle">
                                        <p><font face="Arial" size="2">&nbsp;</font></p>
                                </td>
                                <td width="128" align="center" valign="top">
                                        <p><font face="Arial" size="2">Internet
                                        Price
                                        Markdown (IPM)</font></p>
                                </td>
                               <!-- <td width="110" align="center" valign="top">
                                        <p><font face="Arial" size="2">Value
                                        Price
                                        = Internet Price +</font></p>
                                </td>
                                <td width="110" align="center" valign="top" bgcolor="#999999">
                                        <p><font face="Arial" size="2">For IT
                                        Only:<br>(DB Value)</font></p>
                                </td>-->
                            </tr>
			     <%
			       if (request("cntid")) = "" then
			           Pre_cntid = 0
			       else
  		       	          Pre_cntid = (request("cntid"))
			       end if

			       inTransit14 =Pre_cntid - ( clng(session("inv0_1"))+clng(session("inv0_2"))+clng(session("inv0_3"))+clng(session("inv0_4")))

			       %>
                            <tr>
                                <td width="82" align="left" valign="middle">
                            <p align="left"><font face="Arial" size="1" color="black">In
                            Transit  </font><font face="Arial" size="1">&nbsp;</font><font color='red' size="1" face="Arial">(<%=inTransit14%>)</font></p>
                                </td>
                                <td width="128" align="center" valign="top">
                            <p align="left"><font face="Arial" size="1" color="black">$<input type="text" name="cb1_6" size="10" value='<% =session("tm1_6")%>'>
                                        </font><font face="Arial" size="4" color="red">**</font><font face="Arial" size="1" color="black"><br>CB=<%=session("cb1_6")%></font></p>
                                </td>
                                <!--<td width="110" align="center" valign="top">
                                        <p><font face="Arial" size="3">&nbsp;$<%="BPP"%></font></p>
                                </td>
                                <td width="110" align="center" valign="top" bgcolor="#999999">
                                        <p><font face="Arial" size="3" color="blue"><b>&nbsp;<% =clng(session("tm1_6"))+clng(session("tm0_4"))%></b></font></p>
                                </td>-->
                            </tr>
                            <tr>
                                <td width="82" align="left" valign="middle">
                            <p align="left"><font face="Arial" size="1" color="black">In
                            Stock </font><font face="Arial" size="1"> &nbsp;</font><font color='red' size="1" face="Arial">(<%=clng(session("inv0_1"))+clng(session("inv0_2"))+clng(session("inv0_3"))+clng(session("inv0_4"))%>)</font></p>
                                </td>
                                <td width="128" align="center" valign="top">
                            <p align="left"><font face="Arial" size="1" color="black">$<input type="text" name="cb1_1" size="10" value='<% =session("tm1_1")%>'></font><font face="Arial" size="4" color="red">**</font><font face="Arial" size="1" color="black"><br>CB=<%=session("cb1_1")%></font></p>
                                </td>
                                <!--<td width="110" align="center" valign="top">
                                        <p><font face="Arial" size="3">&nbsp;$<%="BPP"%></font></p>
                                </td>
                                <td width="110" align="center" valign="top" bgcolor="#999999">
                                        <p><font face="Arial" size="3">&nbsp;<% =clng(session("tm1_1"))+clng(session("tm0_4"))%></font></p>
                                </td>-->
                            </tr>
<!--                            <tr>
                                <td width="82" align="left" valign="middle">
                            <p align="left"><font face="Arial" size="1" color="black">31
                         - 60 Days  </font><font face="Arial" size="1">&nbsp;</font><font color='red' size="1" face="Arial">(<%=session("inv0_2")%>)</font></p>
                                </td>
                                <td width="128" align="center" valign="top">
                            <p align="left"><font face="Arial" size="1" color="black">$<input type="text" name="cb1_2" size="10" value='<% =session("tm1_2")%>'></font><font face="Arial" size="4" color="red">**</font><font face="Arial" size="1" color="black"><br>CB=<%=session("cb1_2")%>&nbsp;</font></p>
                                </td>
                                <td width="110" align="center" valign="top">
                                        <p><font face="Arial" size="3">&nbsp;$<%="BPP"%></font></p>
                                </td>
                                <td width="110" align="center" valign="top" bgcolor="#999999">
                                        <p><font face="Arial" size="3">&nbsp;<% =clng(session("tm1_2"))+clng(session("tm0_4"))%></font></p>
                                </td>
                            </tr>
                            <tr>
                                <td width="82" align="left" valign="middle" height="44">
                            <p align="left"><font face="Arial" size="1" color="black">61
                         - 90 Days  </font><font face="Arial" size="1">&nbsp;</font><font color='red' size="1" face="Arial">(<%=session("inv0_3")%>)</font></p>
                                </td>
                                <td width="128" align="center" valign="top" height="44">
                            <p align="left"><font face="Arial" size="1" color="black">$<input type="text" name="cb1_3" size="10" value='<% =session("tm1_3")%>'></font><font face="Arial" size="4" color="red">**</font><font face="Arial" size="1" color="black"><br>CB=<%=session("cb1_3")%>&nbsp;</font></p>
                                </td>
                                <td width="110" align="center" valign="top" height="44">
                                        <p><font face="Arial" size="3">&nbsp;$<%="BPP"%></font></p>
                                </td>
                                <td width="110" align="center" valign="top" bgcolor="#999999" height="44">
                                        <p><font face="Arial" size="3">&nbsp;<% =clng(session("tm1_3"))+clng(session("tm0_4"))%></font></p>
                                </td>
                            </tr>
                            <tr>
                                <td width="82" align="left" valign="middle">
                            <p align="left"><font face="Arial" size="1" color="black">91
                             + Days  </font><font face="Arial" size="1">&nbsp;</font><font color='red' size="1" face="Arial">(<%=session("inv0_4")%>)</font></p>
                                </td>
                                <td width="128" align="center" valign="top">
                            <p align="left"><font face="Arial" size="1" color="black">$<input type="text" name="cb1_4" size="10" value='<% =session("tm1_4")%>'></font><font face="Arial" size="4" color="red">**</font><font face="Arial" size="1" color="black"><br>CB=<%=session("cb1_4")%>&nbsp;</font></p>
                                </td>
                                <td width="110" align="center" valign="top">
                                        <p><font face="Arial" size="3">&nbsp;$<%="BPP"%></font></p>
                                </td>
                                <td width="110" align="center" valign="top" bgcolor="#999999">
                                        <p><font face="Arial" size="3">&nbsp;<% =clng(session("tm1_4"))+clng(session("tm0_4"))%></font></p>
                                </td>
                            </tr>-->
                            <tr>
                                <td width="82" align="left" valign="middle">
                                    <p> <font face="Arial" size="1" color="black">Markdowns
                                        &nbsp;based on:</font></p>
                                </td>
                                <td width="128" align="left" valign="top">
                                    <p><input type="radio" name="cb1_5" value="M" <%=msrp1%>><font face="Arial" size="1" color="black">Over
                                        MSRP</font><br><input type="radio" name="cb1_5" value="I" <%=msrp2%>><font face="Arial" size="1" color="black">Over
                                        INVOICE<br></font></p>
                                </td>
                                <!--<td width="224" align="center" valign="top" colspan="2">
                        <p align="center"><font face="Arial" size="2" color="aqua"><input type="submit" name="submit" value="RECALCULATE" style="font-family:Arial; font-size:x-small;"></font></p>
                                </td>-->
                            </tr>
                        </table>
                    </td>
                    <td width="251" align="center" valign="top"><table border="1" width="238" bgcolor="aqua">
                            <tr>
                                <td align="left" valign="top" colspan="2" bgcolor="#669999" width="228">
                                    <p align="center"><a href="pricematrixedit1.asp?fx=XCLEAR2"><font face="Arial" size="2" color="black">Clear </font></a><font face="Arial" size="2" color="white">&nbsp;&nbsp;Consumer
                                    Rebate</font></p>
                                </td>
                            </tr>
                            <tr>
                                <td width="107" align="left" valign="middle">
                            <p align="left"><font face="Arial" size="1" color="black">Rebate
                                    AMT.</font></p>
                                </td>
                                <td width="115" align="left" valign="top">
                            <p align="left"><font face="Arial" size="1" color="black">$<input type="text" name="cb2_1" size="10" value='<% =session("tm2_1")%>'><br>CB=<%=session("cb2_1")%>&nbsp;</font></p>
                                </td>
                            </tr>
                            <tr>
                                <td width="107" align="left" valign="middle">
                            <p align="left"><font face="Arial" size="1" color="black">&nbsp;Start
                                    Date<br></font><font face="Arial" size="1" color="blue">(
                                        mm/dd/yyyy )</font></p>
                                </td>
                                <td width="115" align="left" valign="top">
                            <p align="left"><font face="Arial" size="1" color="black"><input type="text" name="cb2_2" size="15" value='<% =session("tm2_2")%>' onFocus="javascript:vDateType='1'" onKeyUp="DateFormat(this,this.value,event,false,'1')" onBlur="DateFormat(this,this.value,event,true,'1')"><br>CB=<%=session("cb2_2")%>&nbsp;</font></p>
                                </td>
                            </tr>
                            <tr>
                                <td width="107" align="left" valign="middle">
                            <p align="left"><font face="Arial" size="1" color="black">&nbsp;End
                                    Date<br></font><font face="Arial" size="1" color="blue">(
                                        mm/dd/yyyy )</font><font face="Arial" size="1" color="black">&nbsp;</font></p>
                                </td>
                                <td width="115" align="left" valign="top">
                            <p align="left"><font face="Arial" size="1" color="black"><input type="text" name="cb2_3" size="15" value='<% =session("tm2_3")%>' onFocus="javascript:vDateType='1'" onKeyUp="DateFormat(this,this.value,event,false,'1')" onBlur="DateFormat(this,this.value,event,true,'1')"><br>CB=<%=session("cb2_3")%>&nbsp;</font></p>
                                </td>
                            </tr>
                            <tr>
                                <td width="107" align="left" valign="middle">
                            <p align="left"><font face="Arial" size="1" color="black">Program
                                    Code</font></p>
                                </td>
                                <td width="115" align="left" valign="top">
                            <p align="left"><font face="Arial" size="1" color="black"><input type="text" name="cb2_4" size="15" value='<% =session("tm2_4")%>'><br>CB=<%=session("cb2_4")%>&nbsp;</font></p>
                                </td>
                            </tr>
                            <tr>
                                <td align="center" valign="middle" colspan="2">
                                    <p><font face="Arial" color="blue" size="2">&nbsp;<b><input type="hidden" name="fx" value="save">
				     </b>&nbsp;</font> </p>
                                </td>
                            </tr>
                        </table>
                    </td>
                    <td width="238" align="left" valign="top"><table border="1" width="238" bgcolor="aqua">
                            <tr>
                                <td align="left" valign="top" colspan="2" bgcolor="#669999" width="228">
                                    <p align="center"><a href="pricematrixedit1.asp?fx=XCLEAR3"><font face="Arial" size="2" color="black">Clear</font></a><font face="Arial" size="2" color="black">
                                        </font><font face="Arial" size="2" color="white">&nbsp;&nbsp;&nbsp;Dealer
                                    Incentive</font></p>
                                </td>
                            </tr>
                            <tr>
                                <td width="110" align="left" valign="top">
                            <p align="left"><font face="Arial" size="1" color="black">&nbsp;AMT
                                    Applied to Price</font></p>
                                </td>
                                <td width="112" align="left" valign="top">
                            <p align="left"><font face="Arial" size="1" color="black">$<input type="text" name="cb3_1" size="10" value='<% =session("tm3_1")%>'><br>CB=<%=session("cb3_1")%>&nbsp;</font></p>
                                </td>
                            </tr>
                            <tr>
                                <td width="110" align="left" valign="top">
                            <p align="left"><font face="Arial" size="1">Minimum
                                    AMT</font></p>
                                </td>
                                <td width="112" align="left" valign="top">
                            <p align="left"><font face="Arial" size="1" color="black">$<input type="text" name="cb3_2" size="10" value='<% =session("tm3_2")%>'><br>CB=<%=session("cb3_2")%>&nbsp;</font></p>
                                </td>
                            </tr>
                            <tr>
                                <td width="110" align="left" valign="top">
                            <p align="left"><font face="Arial" size="1">Maximum
                                    AMT</font></p>
                                </td>
                                <td width="112" align="left" valign="top">
                            <p align="left"><font face="Arial" size="1" color="black">$<input type="text" name="cb3_3" size="10" value='<% =session("tm3_3")%>'><br>CB=<%=session("cb3_3")%>&nbsp;</font></p>
                                </td>
                            </tr>
                            <tr>
                                <td width="110" align="left" valign="top" height="26">
                            <p align="left"><font face="Arial" size="1">Start
                                    Date<br></font><font face="Arial" size="1" color="blue">(
                                        mm/dd/yyyy )</font><font face="Arial" size="1">&nbsp;</font></p>
                                </td>
                                <td width="112" align="left" valign="top" height="26">
                            <p align="left"><font face="Arial" size="1" color="black"><input type="text" name="cb3_4" size="10" value='<% =session("tm3_4")%>' onFocus="javascript:vDateType='1'" onKeyUp="DateFormat(this,this.value,event,false,'1')" onBlur="DateFormat(this,this.value,event,true,'1')"><br>CB=<%=session("cb3_4")%>&nbsp;</font></p>
                                </td>
                            </tr>
                            <tr>
                                <td width="110" align="left" valign="top">
                                    <p><font face="Arial" size="1">End Date<br></font><font face="Arial" size="1" color="blue">(
                                        mm/dd/yy yy)</font><font face="Arial" size="1">&nbsp;</font></p>
                                </td>
                                <td width="112" align="left" valign="top">
                            <p align="left"><font face="Arial" size="1" color="black"><input type="text" name="cb3_5" size="10" value='<% =session("tm3_5")%>' onFocus="javascript:vDateType='1'" onKeyUp="DateFormat(this,this.value,event,false,'1')" onBlur="DateFormat(this,this.value,event,true,'1')"><br>CB=<%=session("cb3_5")%>&nbsp;</font></p>
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr>
                    <td width="453" align="center" valign="top">
                        <p align="center"><font face="Arial" size="2" color="aqua"><input type="submit" name="submit" value="CLEAR1" style="font-family:Arial; font-size:x-small;">
                            <input type="submit" name="submit" value="COPY1" style="font-family:Arial; font-size:x-small;">
                            <input type="submit" name="submit" value="PASTE1" style="font-family:Arial; font-size:x-small;"><br><img src="../images/clipboard.gif" width="120" height="10" border="0">&nbsp;</font></p>
                    </td>
                    <td width="251" align="center" valign="top">
                        <p align="center"><font face="Arial" size="2" color="aqua"><input type="submit" name="submit" value="CLEAR2" style="font-family:Arial; font-size:x-small;">
                            <input type="submit" name="submit" value="COPY2" style="font-family:Arial; font-size:x-small;">
                            <input type="submit" name="submit" value="PASTE2" style="font-family:Arial; font-size:x-small;"><br><img src="../images/clipboard.gif" width="120" height="10" border="0">&nbsp;</font></p>
                    </td>
                    <td width="238" align="center" valign="top">
                        <p align="center"><font face="Arial" size="2" color="aqua"><input type="submit" name="submit" value="CLEAR3" style="font-family:Arial; font-size:x-small;">
                            <input type="submit" name="submit" value="COPY3" style="font-family:Arial; font-size:x-small;">
                            <input type="submit" name="submit" value="PASTE3" style="font-family:Arial; font-size:x-small;"><br><img src="../images/clipboard.gif" width="120" height="10" border="0">&nbsp;</font></p>
                    </td>
                </tr>
            </table>
        </td>
    </tr>
    <tr>
        <!--<td colspan="1" width="428" height="333" align="left" valign="top" bgcolor="white" style="margin:auto;border:solid; width:50%">
                <p align="center"><font face="Arial" size="4" color="red"><b>**&nbsp;Important
                Change Notes ! **<br>Internet Price Markdown Values !</b></font></p>
                <p align="left"><font face="Arial" size="2" color="black">The
                value you enter&nbsp;in the </font><font face="Arial" size="3" color="black"><b><i>Internet
                Price Markdown Column</i></b></font><font face="Arial" size="2" color="black">
                has been changed!!</font></p>
                <p align="left"><font face="Arial" size="2" color="black">The
                &nbsp;number you enter now is used to <b>Calculate the Internet
                Price</b> (NOT the value price)<br>(The following examples assume
                you have selected &quot;Markdowns based on: Over INVOICE&quot;)<br></font></p>
                <p align="left"><font face="Arial" size="2" color="black">Example(1)
                - If you want the <b>Internet Pric</b>e of a vehicle to be the
                same as the Invoice price - enter 0 in this column.&nbsp;The
                <b>Value Price</b> (In this case) will later be&nbsp;computed
                to be the<b>&nbsp;Internet Price</b> + $<%="BPP"%>..</font></p>
                <p align="left"><font face="Arial" size="2" color="black">Example(2)
                - If you want the <b>Internet Price</b> of a vehicle to be $300
                UNDER&nbsp;the Invoice price - enter &nbsp;-300 (Minus 300)&nbsp;in
                this column.&nbsp;The <b>Value Price</b> (In this case) will
                later&nbsp;be&nbsp;computed to be the<b>&nbsp;Internet Price</b>
                + $<%="BPP"%>..</font></p>
                <p align="left"><font face="Arial" size="2" color="red">NOTE!</font><font face="Arial" size="2" color="black">
                For your comfort level, the number shown in the &quot;IT Only&quot;
                column is the number you would have entered previously on the
                form. For the more curious, this is also the number stored in
                the database. (Click &quot;RECALCULATE&quot; to update these
                numbers for viewing.)</font></p>
        </td>-->
               <td width="238" align="center" valign="top">
<%IF(1=2) THEN %>
		 <table border="1" width="238" bgcolor="aqua" >
                        <tr>
                            <td align="left" valign="top" colspan="2" bgcolor="#669999" width="228">
                                <p align="center"><a href="pricematrixedit1.asp?fx=XCLEAR6"><font face="Arial" size="2" color="black">Clear</font></a><font face="Arial" size="2" color="black">
                                    </font><font face="Arial" size="2" color="white">&nbsp;&nbsp;&nbsp;Buy</font></p>
                            </td>
                        </tr>
                        <tr>
                            <td width="110" align="left" valign="top">
                        <p align="left"><font face="Arial" size="1" color="black">
                                Interest Rate</font></p>
                            </td>
                            <td width="112" align="left" valign="top">
                        <p align="left"><font face="Arial" size="1" color="black">%<input type="text" name="cb6_1" size="10" value='<% =session("tm6_1")%>' onBlur="updateBuyDisclaimerRate(this.value)"><br>CB=<%=session("cb6_1")%>&nbsp;</font></p>
                            </td>
                        </tr>
                        <tr>
                            <td width="110" align="left" valign="top">
                        <p align="left"><font face="Arial" size="1">Down Payment</font></p>
                            </td>
                            <td width="112" align="left" valign="top">
                        <p align="left"><font face="Arial" size="1" color="black">$<input type="text" name="cb6_2" size="10" value='<% =session("tm6_2")%>' onBlur="updateBuyDisclaimerDownPayment(this.value)"><br>CB=<%=session("cb6_2")%>&nbsp;</font></p>
                            </td>
                        </tr>
                        <tr>
                            <td width="110" align="left" valign="top">
                        <p align="left"><font face="Arial" size="1">Term</font></p>
                            </td>
                            <td width="112" align="left" valign="top">
                        <p align="left"><font face="Arial" size="1" color="black">%<input type="text" name="cb6_3" size="10" value='<% =session("tm6_3")%>' onBlur="updateBuyDisclaimerTerm(this.value)"><br>CB=<%=session("cb6_3")%></font></p>
                            </td>
                        </tr>
			<tr>
                                <td width="110" align="left" valign="top" height="26">
                            <p align="left"><font face="Arial" size="1">Start
                                    Date<br></font><font face="Arial" size="1" color="blue">(mm/dd/yyyy)</font><font face="Arial" size="1">&nbsp;</font></p>
                                </td>
                                <td width="112" align="left" valign="top" height="26">
                            <p align="left"><font face="Arial" size="1" color="black"><input type="text" name="cb6_7" size="10" value='<% =session("tm6_7")%>' onFocus="javascript:vDateType='1'" onKeyUp="DateFormat(this,this.value,event,false,'1')" onBlur="DateFormat(this,this.value,event,true,'1')"><br>CB=<%=session("cb6_7")%>&nbsp;</font></p>
                                </td>
                            </tr>
                            <tr>
                                <td width="110" align="left" valign="top">
                                    <p><font face="Arial" size="1">End Date<br></font><font face="Arial" size="1" color="blue">(mm/dd/yyyy)</font><font face="Arial" size="1">&nbsp;</font></p>
                                </td>
                                <td width="112" align="left" valign="top">
                            <p align="left"><font face="Arial" size="1" color="black"><input type="text" name="cb6_8" size="10" value='<% =session("tm6_8")%>' onFocus="javascript:vDateType='1'" onKeyUp="DateFormat(this,this.value,event,false,'1')" onBlur="DateFormat(this,this.value,event,true,'1')"><br>CB=<%=session("cb6_8")%>&nbsp;</font></p>
                                </td>
                        </tr>

               <tr>
                  <td width="110" align="left" valign="top">
                     <p align="left"><font face="Arial" size="1">Buy Disclaimer:</font></p>
                         </td>
                         <td width="112" align="left" valign="top">
                     <p align="left"><font face="Arial" size="1" color="black"><textarea  rows="6" cols="30" id="cb6_9" name="cb6_9" placeholder="Payments based on an Interest Rate of <> for <> months, with $<> down"><% =session("tm6_9")%></textarea><br>CB=<%=session("cb6_9")%>&nbsp;</font></p>
                 </td>
              </tr>

                       <tr>
                           <td width="224" align="center" valign="top" colspan="2">
                                <p align="center"><font face="Arial" size="1" color="black"><strong>Calculate Payments to see on Fitzmall</strong> <input type="checkbox" name="cb6_6" <%=showPayment%>></font></p>
                           </td>
                       </tr>
                       <tr>
                           <td width="224" align="center" valign="top" colspan="2">
                                <p align="center"><font face="Arial" size="1" color="black"><strong><i>IN LIEU</i> of Consumer Rebate</strong> <input type="checkbox" name="cb6_4" value="<%=session("tm6_4")%>" <%=IIF(session("tm6_4") = "1","checked","")%>></font></p>
                           </td>
                       </tr>

                    </table>
<%end if%>

                </td>
        <td width="238" align="center" valign="top">

<%IF(g_enableLeasing = "true") THEN %>
	   <table border="1" width="238" bgcolor="aqua" >
                <tr>
                    <td align="left" valign="top" colspan="2" bgcolor="#669999" width="228">
                        <p align="center"><a href="pricematrixedit1.asp?fx=XCLEAR5"><font face="Arial" size="2" color="black">Clear</font></a><font face="Arial" size="2" color="black">
                            </font><font face="Arial" size="2" color="white">&nbsp;&nbsp;&nbsp;Lease</font></p>
                    </td>
                </tr>
                <tr>
                    <td width="110" align="left" valign="top">
                <p align="left"><font face="Arial" size="1" color="black">
			<input type="radio" name="rateGroup" id="interestRate" value="interest" <%=isGM%>>Interest Rate (GM)</input>
			<input type="radio" name="rateGroup" id="moneyFactor" value="factor" <%=notGM%>>Money Factor</input>
			</font></p>
                    </td>
                    <td width="112" align="left" valign="top">
                <p align="left"><font face="Arial" size="1" color="black"><input type="text" id="cb5_1" name="cb5_1" size="10" value='<% =session("tm5_1")%>'><br>CB=<%=session("cb5_1")%>&nbsp;</font></p>
                    </td>
                </tr>
                        <tr>
                            <td width="110" align="left" valign="top">
                        <p align="left"><font face="Arial" size="1">Term (months)</font></p>
                            </td>
                            <td width="112" align="left" valign="top">
                        <p align="left"><font face="Arial" size="1" color="black">M<input type="text" name="cb5_11" size="10" value='<% =session("tm5_11")%>' onBlur="updateDisclaimerTerm(this.value)"><br>CB=<%=session("cb5_11")%></font></p>
                            </td>
                        </tr>

                        <tr>
                            <td width="110" align="left" valign="top">
                        <p align="left"><font face="Arial" size="1">Money Down</font></p>
                            </td>
                            <td width="112" align="left" valign="top">
                        <p align="left"><font face="Arial" size="1" color="black">$<input type="text" name="cb5_12" size="10" value='<% =session("tm5_12")%>' onBlur="updateDisclaimerDownPayment(this.value)"><br>CB=<%=session("cb5_12")%>&nbsp;</font></p>
                            </td>
                        </tr>


                <!--<tr>
                    <td width="110" align="left" valign="top">
                <p align="left"><font face="Arial" size="1">Rate Markup</font></p>
                    </td>
                    <td width="112" align="left" valign="top">
                <p align="left"><font face="Arial" size="1" color="black">%<input type="text" name="cb5_2" size="10" value='<% =session("tm5_2")%>'><br>CB=<%=session("cb5_2")%>&nbsp;</font></p>
                    </td>
                </tr>-->
                <input type="hidden" name="cb5_2" size="10" value='<% =session("tm5_2")%>'>
                <tr>
                    <td width="110" align="left" valign="top">
                <p align="left"><font face="Arial" size="1">Lease Rebate</font></p>
                    </td>
                    <td width="112" align="left" valign="top">
                <p align="left"><font face="Arial" size="1" color="black">$<input type="text" name="cb5_3" size="10" value='<% =session("tm5_3")%>'><br>CB=<%=session("cb5_3")%>&nbsp;</font></p>
                    </td>
                </tr>
                <tr>
                    <td width="110" align="left" valign="top">
                <p align="left"><font face="Arial" size="1">Dealer Lease Incentive</font></p>
                    </td>
                    <td width="112" align="left" valign="top">
                <p align="left"><font face="Arial" size="1" color="black">$<input type="text" name="cb5_20" size="10" value='<% =session("tm5_20")%>'><br>CB=<%=session("cb5_20")%>&nbsp;</font></p>
                    </td>
                </tr>
                <tr>
                    <td width="110" align="left" valign="top">
	                <p align="left"> <!--<font face="Arial" size="1">Residual Value(15k)</font>-->
				<input type="radio" name="residualValueGroup" id="residualValue15k" value="15K" onclick="updateTextBoxes(this.value)" <%=selected15K%>><font face="Arial" size="1">15K Residual Value</font></input> 
			</p>
                    </td>
                    <td width="112" align="left" valign="top">
                <p align="left"><font face="Arial" size="1" color="black">%<input type="text" id="cb5_4" name="cb5_4" size="10" value='<% =session("tm5_4")%>'><br>CB=<%=session("cb5_4")%></font></p>
                    </td>
                </tr>
                <tr>
                    <td width="110" align="left" valign="top">
                	<p align="left"><!--<font face="Arial" size="1">10K Residual Value Enhanced</font>-->
			        <input type="radio" name="residualValueGroup" id="residualValue10k" value="10K" onclick="updateTextBoxes(this.value)" <%=selected10K%>><font face="Arial" size="1">10K Residual Value</font></input>                    
			</p>
		    </td>
                    <td width="112" align="left" valign="top">
                <p align="left"><font face="Arial" size="1" color="black">%<input type="text" id="cb5_5" name="cb5_5" size="10" value='<% =session("tm5_5")%>'><br>CB=<%=session("cb5_5")%></font></p>
                    </td>
                </tr>
               <tr>
                   <td width="110" align="left" valign="top">
	               <p align="left"><!--<font face="Arial" size="1">12K Residual Value Enhanced</font>-->
				<input type="radio" name="residualValueGroup" id="residualValue12k" value="12K" onclick="updateTextBoxes(this.value)" <%=selected12K%>><font face="Arial" size="1">12K Residual Value</font></input> 
			</p>
                   </td>
                   <td width="112" align="left" valign="top">
               <p align="left"><font face="Arial" size="1" color="black">%<input type="text" id="cb5_6" name="cb5_6" size="10" value='<% =session("tm5_6")%>'><br>CB=<%=session("cb5_6")%></font></p>
                   </td>
               </tr>

               <tr>
                   <td width="110" align="left" valign="top">
               <p align="left"><font face="Arial" size="1">Bank Fee (Acquisition Fee)</font></p>
                   </td>
                   <td width="112" align="left" valign="top">
               <p align="left"><font face="Arial" size="1" color="black">$<input type="text" name="cb5_7" size="10" value='<% =session("tm5_7")%>'><br>CB=<%=session("cb5_7")%>&nbsp;</font></p>
                   </td>
               </tr>

<%'if g_loc = "CJE" then ' 8/8/2018 SHOW ALL LOCATION
   session("tm5_9") = 400   '8/7/2018
 %>
               <tr>
                   <td width="110" align="left" valign="top">
                      <p align="left"><font face="Arial" size="1">Lease Fee: $</font></p>
                          </td>
                          <td width="112" align="left" valign="top">
                      <p align="left"><font face="Arial" size="1" color="black">$<input DISABLED type="text" name="cb5_9" size="10" value='<% =session("tm5_9")%>'><br>CB=<%=session("cb5_9")%>&nbsp;</font></p>
                  </td>
               </tr>
<%'end if%>

		<tr>
                                <td width="110" align="left" valign="top" height="26">
                            <p align="left"><font face="Arial" size="1">Excess Mileage Charge</font></p>
                                </td>
                                <td width="112" align="left" valign="top" height="26">
                            <p align="left"><font face="Arial" size="1" color="black">$<input type="text" name="cb5_18" size="10" value='<% =session("tm5_18")%>' onBlur="updateDisclaimerExcessMileage(this.value);"><br>CB=<%=session("cb5_18")%>&nbsp;</font></p>
                                </td>
                            </tr>
                            <tr>
                                <td width="110" align="left" valign="top">
                                    <p><font face="Arial" size="1">Disposition Fee</font></p>
                                </td>
                                <td width="112" align="left" valign="top">
                            <p align="left"><font face="Arial" size="1" color="black">$<input type="text" name="cb5_19" size="10" value='<% =session("tm5_19")%>' onBlur="updateDisclaimerDispositionFee(this.value);"><br>CB=<%=session("cb5_19")%>&nbsp;</font></p>
                                </td>
                        </tr>


		<tr>
                                <td width="110" align="left" valign="top" height="26">
                            <p align="left"><font face="Arial" size="1">Start
                                    Date<br></font><font face="Arial" size="1" color="blue">(mm/dd/yyyy)</font><font face="Arial" size="1">&nbsp;</font></p>
                                </td>
                                <td width="112" align="left" valign="top" height="26">
                            <p align="left"><font face="Arial" size="1" color="black"><input type="text" name="cb5_15" size="10" value='<% =session("tm5_15")%>' onFocus="javascript:vDateType='1'" onKeyUp="DateFormat(this,this.value,event,false,'1')" onBlur="DateFormat(this,this.value,event,true,'1')"><br>CB=<%=session("cb5_15")%>&nbsp;</font></p>
                                </td>
                            </tr>
                            <tr>
                                <td width="110" align="left" valign="top">
                                    <p><font face="Arial" size="1">End Date<br></font><font face="Arial" size="1" color="blue">(mm/dd/yyyy)</font><font face="Arial" size="1">&nbsp;</font></p>
                                </td>
                                <td width="112" align="left" valign="top">
                            <p align="left"><font face="Arial" size="1" color="black"><input type="text" name="cb5_16" size="10" value='<% =session("tm5_16")%>' onFocus="javascript:vDateType='1'" onKeyUp="DateFormat(this,this.value,event,false,'1')" onBlur="DateFormat(this,this.value,event,true,'1');updateDisclaimerDeliveryDate(this.value);"><br>CB=<%=session("cb5_16")%>&nbsp;</font></p>
                                </td>
                        </tr>

               <tr>
                  <td width="110" align="left" valign="top">
                     <p align="left"><font face="Arial" size="1">Lease Disclaimer:</font></p>
                         </td>
                         <td width="112" align="left" valign="top">


                     <p align="left"><font face="Arial" size="1" color="black"><textarea  rows="6" cols="30" id="cb5_10" name="cb5_10" placeholder="Lease payments based on <> month, <> miles per year. <> down. Plus first month lease payment, refundable security deposit taxes, tags and $<%=processing_fee%> dealer processing fee. Does not include  <> disposition fee due at lease end. <> per mile over allowance. Customer responsible for excess wear and tear. On approved credit, not all customers will qualify. Must take delivery before <>."><% =session("tm5_10")%></textarea><br>CB=<%=session("cb5_10")%>&nbsp;</font></p>
                 </td>
              </tr>
                       <tr>
                           <td width="224" align="center" valign="top" colspan="2">
                                <p align="center"><font face="Arial" size="1" color="black"><strong>Calculate Lease Payments to see on Fitzmall</strong> <input type="checkbox" name="cb5_14" <%=showLease%>></font></p>
                           </td>
                       </tr>

               <!--<tr>
                   <td width="224" align="center" valign="top" colspan="2">
                        <p align="center"><font face="Arial" size="1" color="black"><strong>Special Lease Program <i>IN LIEU</i> of Consumer Rebate</strong> <input type="checkbox" name="cb5_8" <%=LieuLease%>></font></p>
                   </td>
               </tr>
               <tr>
                   <td width="224" align="center" valign="top" colspan="2">
                        <p align="center"><font face="Arial" size="2" color="aqua"><input type="button" name="recalclease" value="RECALCULATE LEASE" style="font-family:Arial; font-size:x-small;"></font></p>
                   </td>
               </tr>-->

            </table>
<% end if %>
        </td>
        <td valign="top" width="294" height="333">

                <div align="left">
                    <p>&nbsp;</p>
                </div>
<p align="center"><a href='pricematrixshow1.asp?lxid=<%=g_id%>&rand=<%=rand%>#<%=g_id%>'><font face="Arial" size="2" color="aqua">BACK
                TO LIST</font></a><font face="Arial" size="2" color="aqua"><br><br> &nbsp;&nbsp;&nbsp;&nbsp;</font><font face="Arial" size="3" color="white"><input type="submit" name="submit" value="SAVE CHANGES !"><br><br>
                &nbsp;</font><a href="../default.asp?rand=<%=Int(99999999 * Rnd())%>"><font face="Arial" size="2" color="aqua">HOME</font></a></p>


	    </font>        </td>
    </tr>
        <tr>
        <td width="238" align="center" valign="top">
<%IF(g_enableLeasing = "true") THEN %>
                                        <p align="center"><font face="Arial" size="2" color="aqua">
                                        <input type="submit" name="submit" value="CLEAR6" style="font-family:Arial; font-size:x-small;">
                                            <input type="submit" name="submit" value="COPY6" style="font-family:Arial; font-size:x-small;">
                                            <input type="submit" name="submit" value="PASTE6" style="font-family:Arial; font-size:x-small;"><br>
                                            <img src="../images/clipboard.gif" width="120" height="10" border="0">&nbsp;</font></p>
<% end if %>
                                    </td>

        <td width="238" align="center" valign="top">
<%IF(g_enableLeasing = "true") THEN %>
                                <p align="center"><font face="Arial" size="2" color="aqua">
                                <input type="submit" name="submit" value="CLEAR5" style="font-family:Arial; font-size:x-small;">
                                    <input type="submit" name="submit" value="COPY5" style="font-family:Arial; font-size:x-small;">
                                    <input type="submit" name="submit" value="PASTE5" style="font-family:Arial; font-size:x-small;"><br>
                                    <img src="../images/clipboard.gif" width="120" height="10" border="0">&nbsp;</font></p>
<% end if %>
                            </td>
        <td width="294">

<p align="center"><a href="http://jjfserver"><font face="Arial" size="3" color="white"><br></font></a>&nbsp;</p>

<input type="hidden" value="<%=g_id%>" name="matrixid"/>
	    </font>        </td>
        </tr>
</table>
</form>

</body>
</html>
<%
'=================================
'SUB - FIND LOWEST INTEREST RATE
'=================================
Sub lowest

  Dim low
  low = CDbl(99999999.99)
  'Response.Write low & "<br>"

    If session("tm4_10") <> "" Then
     If CDbl(session("tm4_10")) < low  Then
      low = CDbl(session("tm4_10"))
     End If
    End If
    If session("tm4_11") <> "" Then
     If CDbl(session("tm4_11")) < low  Then
      low = CDbl(session("tm4_11"))
     End If
    End If
    If session("tm4_12") <> "" Then
     If CDbl(session("tm4_12")) < low  Then
      low = CDbl(session("tm4_12"))
     End If
    End If
    If session("tm4_13") <> "" Then
     If CDbl(session("tm4_13")) < low  Then
      low = CDbl(session("tm4_13"))
     End If
    End If
    If session("tm4_14") <> "" Then
     If CDbl(session("tm4_14")) < low  Then
      low = CDbl(session("tm4_14"))
     End If
    End If
    If session("tm4_15") <> "" Then
     If CDbl(session("tm4_15")) < low  Then
      low = CDbl(session("tm4_15"))
     End If
    End If
	 session("tm4_3") = low
    If low = CDbl(99999999.99) Then
	 session("tm4_3") = "N/A"
    End If
    'Response.Write low

End Sub
'=================================
%>
<%
SUB SENDMAIL()
'SUB SENDMAIL(Location,	Make,ModelNumber,OldMarkup,NewMarkup,AgeRange,Associate)


dim ArrVal(20)
   location =""
   AgeRange =""
   make = ""
   ModelNumber = ""
   Associate =""
   VAL0=""
   VAL1=""
   VAL2=""
   VAL3=""
   VAL4=""
   rebate3  =  ""
   rebate4  =  ""
  Applied3   = ""
  Applied4   = ""
  tmpmsrp   =  ""  'nothing change

Set rs01 = Server.CreateObject("ADODB.Recordset")

emailcol = "ID,loc,MAKE_code as make,MODEL,markup AS oldmarkup, UPD_markup,invtot,UPUSER,cramt,UPD_cramt,fdamt,UPD_fdamt,markupit,UPD_markupit,markup30,UPD_markup30,markup60, UPD_markup60,markup90,UPD_markup90,msrp,upd_msrp"
	SQLStmt3 = "SELECT " & emailCol
	SQLStmt3 = SQLStmt3 & " from foxprotables.dbo.RImast_UPD  "
	SQLStmt3 = SQLStmt3 & " WHERE ID ='" &  session("key") &"' AND  (FLAG IS  NULL  OR FLAG  ='') "
       ' RESPONSE.WRITE  "<BR>EMAIL==" & SQLStmt3
        rs01.Open SQLStmt3, Conn, 2,2


'####COMPARE VALUE HAS BEEN CHANGED ##############
' SPLITVAL(0) =header1
' SPLITVAL(1) =header2
' ' VAL2=SPLITVAL(2)  'old value
  ' VAL3=SPLITVAL(3)  'new value
   'VAL4=SPLITVAL(4)  'AGE RANGE
'#################################################
IF clng(RS01("oldmarkup"))  <> clng(RS01("UPD_markup")) then ArrVal(0) ="Old Markup|New. Markup|"& RS01("oldmarkup") &"|"&  RS01("UPD_markup")&"|"& "00 - 30 Days"        '00-30 days
IF clng(RS01("markupit"))  <> clng(RS01("UPD_markupit")) then ArrVal(1) ="Old MarkupIT|New. MarkupIT|"& RS01("markupit") &"|"&  RS01("UPD_markupit")&"|"& "IT"
IF clng(RS01("markup30"))  <> clng(RS01("UPD_markup30")) then ArrVal(2) ="Old Markup30|New.Markup30|"&RS01("markup30") &"|"&  RS01("UPD_markup30")&"|"& "31 - 60 Days"
IF clng(RS01("markup60"))  <> clng(RS01("UPD_markup60")) then ArrVal(3) ="Old Markup60|New.Markup60|"&RS01("markup60") &"|"&  RS01("UPD_markup60")&"|"& "61 - 90 Days"
IF clng(RS01("markup90"))  <> clng(RS01("UPD_markup90")) then ArrVal(4) ="Old Markup90|New. Markup90|"&RS01("markup90") &"|"&  RS01("UPD_markup90")&"|"& "91 + Days"
IF clng(RS01("cramt"))     <> clng(RS01("UPD_cramt")) 	 then  ArrVal(5) ="Old Rebate AMT.|New. Rebate AMT.|"&RS01("cramt") &"|"&  RS01("UPD_cramt")&"|"& "Rebate"
IF clng(RS01("fdamt"))     <> clng(RS01("UPD_fdamt")) 	 then  ArrVal(6) ="Old AMT Applied to Price|New AMT Applied to Price|"&RS01("fdamt") &"|"&  RS01("UPD_fdamt")&"|"& "Applied"
IF RS01("msrp")    <>  RS01("UPD_msrp") then  ArrVal(7) ="Old msrp|New msrp|"&RS01("msrp") &"|"&  RS01("UPD_msrp")&"|"& "msrp"


SHOWME =""
for i = 0 to ubound(ArrVal)
   IF TRIM(ArrVal(i)) <> "" then
       SPLITVAL = split(ArrVal(i),"|")
     ' response.write "<br>"& i & rs01("loc") & "===="    &  ArrVal(i)

      if  i < 5 then
         VAL2=SPLITVAL(2)  'old value
         VAL3=SPLITVAL(3)  'new value
         VAL4=SPLITVAL(4)  'AGE RANGE
      end if
      IF i = 5 then
         rebate3  =  SPLITVAL(2)
	 rebate4  =  SPLITVAL(3)
       end if
      IF i = 7 then  tmpmsrp   =  RS01("UPD_msrp")
      IF i = 6 then
        Applied3   = SPLITVAL(2)
        Applied4   = SPLITVAL(3)
      end if
      SHOWME ="T"
   ELSE
     'EMPTY SAME VALUE
   end if

next

'response.end


IF SHOWME="T" THEN

 'response.write "<br> SOME CHANGED ====" & rs01("loc")  & "/"& SHOWME

   location =rs01("loc")
   AgeRange =rs01("invtot")
   make = rs01("make")
   ModelNumber = rs01("model")
   Associate =rs01("upuser")
   VAL0= "Old Markup" 'SPLITVAL(0) 'header not use now
   VAL1="New Markup" 'SPLITVAL(1)  'header not use now


'response.write "<br> tmpmsrp  ====" & rs01("loc")  & "/"& tmpmsrp




user_name = session("login")
email = session("emailBB")
phone =  session("phoneBB")

'& vbcrlf

HTML = "<!DOCTYPE HTML PUBLIC""-//IETF//DTD HTML//EN"">"
HTML = HTML & "<html>"
HTML = HTML & "<head>"
HTML = HTML & "<title>Sending CDONTS Email Using HTML</title>"
HTML = HTML & "</head>"
HTML = HTML & "<body bgcolor='#E7E7DE'><span style='font-size:10pt;'><font face='Verdana'>"
HTML = HTML & "The following Price Matrix changes have been made: "  & "<br><br>"
'HTML = HTML & "  Date: " & now() & "<BR>"
'HTML = HTML & "  Modified By: " & session("name") & "<br>"
'HTML = HTML & "  Phone: " & phone & "<br>"
'HTML = HTML & "  Email: " & "<a href='mailto:" & email & "'>" &email& "</a><br>"
'HTML = HTML & "  Description:  " & Location &"-" &Make & "</font></span><br>"
'HTML = HTML & "<br>"
HTML = HTML & "<table align='left' border='1' cellspacing='0' width='400' bordercolordark='white' bordercolorlight='black'>"

HTML = HTML & "<tr bgcolor='silver'>"
HTML = HTML & "<span style='font-size:8pt;'><font face='Verdana'>"
HTML = HTML &"<td><b>Location</b></td><td><b>Make</b></td><td nowrap><b>Model Number</b></td><td nowrap><b>" &  VAL0 &"</b></td><td nowrap><b>" &  VAL1 &"</b></td><td nowrap><b>Age Range</b></td>"
HTML = HTML &"<td ><b>Old CR</b></td><td ><b>New CR</b></td><td ><b>Old DI</b></td><td ><b>New DI</b></td><td ><b>Markup Based on</b></td>"
HTML = HTML &"<td ><b>Associate</b></td><td nowrap><b>Time & Date</b></td>"
HTML = HTML & "</font></span>"
HTML = HTML & "</tr>"
HTML = HTML & "<tr bgcolor='white'>"
HTML = HTML & "<span style='font-size:8pt;'><font face='Verdana'>"
HTML = HTML &"<td><b>"&Location&"</b></td><td><b>"&Make&"</b></td><td><b>"&ModelNumber&"</b></td><td><b>"&VAL2&"</b></td><td><b>"&VAL3&"</b></td><td><b>"&VAL4&"</b></td>"
HTML = HTML &"<td><b>"&Rebate3&"</b></td><td><b>"&Rebate4&"</b></td>"
HTML = HTML &"<td><b>"&Applied3&"</b></td><td><b>"&Applied4&"</b></td><td><b>"&tmpmsrp&"</b></td>"
HTML = HTML &"<td><b>"&Associate&"</b></td><td nowrap><b>"&now()&"</b></td>"
HTML = HTML & "</font></span>"
HTML = HTML & "</tr>"
HTML = HTML & "</table><br><br><BR><BR>"

HTML = HTML & "<br><br><br><span style='font-size:10pt;'><font face='verdana'>"
HTML = HTML & "Thank You,<br><br>"
HTML = HTML & session("name")
'HTML = HTML & "<br><br>  To Check the listing, Please CLICK on The Following LINK <br>"
'HTML = HTML & "    <a href='http://jjfserver/asp/pricematrixedit1.asp?fx=edit" & "&id=" & session("key") &"&rand="& Rand &"&mc=" & "&loc=" &location&"&cntid=" &session("cntid") & "'>" &"Price Matrix Review</a>"
HTML = HTML & "<br>"
HTML = HTML &  "</font></span>"

HTML = HTML & "</body>"
HTML = HTML & "</html>"

 

Dim offMgr
offMgr = location

Select Case offMgr 
	Case "CDO"
   	   'ORIGINAL  mailTo =  "aschers@fitzmall.com, lewisg@fitzmall.com; goodwink@fitzmall.com; cashb@fitzmall.com;washingtone@fitzmall.com; jamesi@fitzmall.com; ascherd@fitzmall.com; guilbeauxj@fitzmall.com"
     mailTo =  "aschers@fitzmall.com, lewisg@fitzmall.com; cashb@fitzmall.com; jamesi@fitzmall.com; guilbeauxj@fitzmall.com;ascherd@fitzmall.com;montalvol@fitzmall.com"

	Case "LFO"
	  mailTo =  "aschers@fitzmall.com, lewisg@fitzmall.com; cashb@fitzmall.com;jamesi@fitzmall.com;guilbeauxj@fitzmall.com;ascherd@fitzmall.com;montalvol@fitzmall.com"
	Case "FBN","FBS"
   	  mailTo = "aschers@fitzmall.com, lewisg@fitzmall.com; cashb@fitzmall.com;jamesi@fitzmall.com;guilbeauxj@fitzmall.com;ascherd@fitzmall.com;montalvol@fitzmall.com"
	Case "WDC"

	Case "FAM","FSS"

        Case "FOC","FMM"
	Case "CJE"

    	Case "FTN"

	Case "FLP"

	Case Else
	     mailTo = "spunch@fitzmall.com"

End Select



' Send The E-mail's
    Dim myMail
    Dim HTML

   ' Set myMail = CreateObject("CDONTS.NewMail")

    mymailfrom=session("emailBB")
    mymailto= mailTo
  '  myMailCC= mailcc
    mymailsubject="Price Matrix has been  modified By " & location&"-"&make
    mymailbody=HTML


'  mymail.From=session("emailBB")
  '  mymail.To= mailTo
 '    myMail.cc= mailcc

 '   mymail.Subject="Used Vehicle Purchase Request Log "
    'myMail.BodyFormat=0
   ' myMail.MailFormat=0
   ' mymail.Body=HTML
    ss = 1
    if ss=1 then
     ' response.write  "<img src='images/requestsentani.gif' border='0'><br>"
      IF SESSION("LOGIN")  ="SPUNCH" THEN
        mymailto= "spunch@fitzmall.com"

      END IF


     ' response.write "<BR>" & HTML    'mymailto & " " & mymailfrom & " " & mymailsubject & " "  & myMailCC  & "<BR>TESTING NOT SEND "
      IF SHOWME ="T"  THEN    ' Value in mark up changed
       'testing mymailto= "spunch@fitzmall.com"  'soi will delete this line
        Call sendCDOemail(mymailto,mymailfrom,mymailsubject,mymailbody,"HTML", "","")
      END IF

    end if
END IF  'showme end


    'set mymail=nothing


END SUB

SUB WriteToTextFile(str)

dim filesys, filetxt
Const ForReading = 1, ForWriting = 2, ForAppending = 8
Set filesys = CreateObject("Scripting.FileSystemObject")
'Set filetxt = filesys.OpenTextFile("C:\Inetpub\wwwroot\production\ASP\MatrixErrorLog.txt", ForAppending, True)
Set filetxt = filesys.OpenTextFile("F:\MatrixErrorLog\MatrixErrorLog.txt", ForAppending, True)
filetxt.WriteLine(Now & " - " & session("login") &  " - " & str)
filetxt.Close 

END SUB

%>