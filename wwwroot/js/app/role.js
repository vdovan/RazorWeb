var tableRoles = document.querySelectorAll('#tableRole>tr');
console.dir(tableRoles);
tableRoles.forEach(role =>{
    console.dir(role);
    role.addEventListener('click', function(e) {
        var txtroleName = document.getElementById("txtroleName");
        txtroleName.value = role.firstElementChild.innerText;
    })
})
var Showdata = function (roleName) {

    var txtroleName = document.getElementById("txtroleName");
    txtroleName.value = roleName;
}

var SaveData = function () {
    var roleName = document.getElementById("txtroleName").value;
    var antiForgeryToken = $("input[name=__RequestVerificationToken]").val()
    $.ajax({
        headers: {
            "__RequestVerificationToken": antiForgeryToken
        },
        type: "POST",
        //url: '@Url.Action("Create","role")',
        url: '/role/create',
        data: { Name: roleName },
        success: function (res) {
            if (res.ret == 1) {
                AddNewRoleToTable(res.msg);
            }

            ShowMessage(res);

        },
        error: function (res) {
            console.log("Create role error", res);
            res.ret = 0;
            res.userMsg = "Unhandle exception error";
            ShowMessage(res);
        }
    })
}

var ShowMessage = function (res) {
    var message = document.getElementById("StatusMessage");
    message.innerHTML = res.userMsg;
    var className = "text-danger";
    if (res.ret == 1) {
        className = "text-success";
    }
    message.classList = className;
    setTimeout(function () {
        console.log("timeout");
        message.innerHTML = "";
    }, 30000);
}

var AddNewRoleToTable = function (role) {
    console.log(role);
    var roleTable = document.getElementById("tableRole");
    var roleNew = document.createElement("tr");

    //role Name data
    var roleName = document.createElement("td");
    roleName.innerHTML = role.name;
    roleNew.appendChild(roleName);

    // delete button
    var roleDel = document.createElement('td');
    var roleDelBtn = document.createElement('a');
    roleDelBtn.classList = "btn btn-danger btn-sm";
    roleDelBtn.href = '/Role/Delete/' + role.id;
    roleDelBtn.innerHTML ='Xóa';
    roleDel.appendChild(roleDelBtn);
    roleNew.appendChild(roleDel);

    roleTable.appendChild(roleNew);

    document.getElementById("txtroleName").value = '';
}
