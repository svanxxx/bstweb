function replaceAll(find, replace, str) {
    return str.replace(new RegExp(find, 'g'), replace);
}

function Rerun(img, ReqestID, CommandName, UserName, TestRunID) {

    var strResponse = '';

    $.ajax({
        type: "POST",
        url: "WebService.asmx/RunTest",
        data: '{strReqestID:"' + ReqestID + '",strCommandName:"' + replaceAll('\\\\','~',CommandName) + '",UserName:"' + UserName + '",TestRunID:"' + TestRunID + '" }',
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (response) {
            strResponse = response.d;
            //alert(strResponse);
            if (strResponse == "OK") { img.src = "Images/Sign_rerun.png"; }
            else { alert("ReRun Error!"); }
        },
        error: function (xhr, ajaxOptions, thrownError) {  alert("Error:" + xhr.responseText);}
    });
}



function ChangeRerun(img, ReqestID, CommandName, UserName, TestRunID) {

    var strResponse = '';
    var newCommandName = replaceAll('`', '"', CommandName);
    var chCommand = prompt(" You reRun command is : " + newCommandName + " \n You can change reRun command:", newCommandName);
    if (chCommand != null) {
        chCommand = replaceAll('"', '`', chCommand);
        Rerun(img, ReqestID, chCommand, UserName, TestRunID);
    }
}


function deleteRow(rowid) {
    var row = document.getElementById(rowid);
    row.parentNode.removeChild(row);
}


function StopSequence(TrName, SEQUENCEGUID, ThosterID) {

    var strResponse = '';

    $.ajax({
        type: "POST",
        url: "WebService.asmx/StopSequence",
        data: '{SEQUENCEGUID:"' + SEQUENCEGUID + '",ThosterID:"' + ThosterID + '" }',
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (response) {
            strResponse = response.d;
            if (strResponse == "OK") {
                deleteRow(TrName);
            }
            else { alert("StopSequence Error!"); }
        },
        error: function (xhr, ajaxOptions, thrownError) { alert("Error:" + xhr.responseText); }
    });
}