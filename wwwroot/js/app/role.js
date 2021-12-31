var roleMgr = function(){
    return {
        roles: null,
        selected: null,
        init: function () {

            var antiForgeryToken = $("input[name=__RequestVerificationToken]").val()
            var getRoles = function(){
                console.log("getRoles");
                $.ajax({
                    headers: {
                        "__RequestVerificationToken": antiForgeryToken
                    },
                    type: 'GET',
                    url: '/role/getRoles',
                    success: function (res) {
                        if(res.ret == 1)
                        { 
                            roles = res.msg;
                            initRoleTable(res.msg);
                        }
                    },
                    error: function (res) {
                        console.log("call api res unhandle exception", res);
                        res.ret = 0;
                        res.userMsg = "Unhandle exception error";
                        ShowMessage(res);
                    }
                })
            }();
            
            $('#searchbtn').click(function(e){
                e.preventDefault();
                var searchBox = document.getElementById('txtRoleSearch');
                var text = searchBox.value.toLowerCase();
                var roleList = GetRoleStore();
                if(text){
                    roleList = roleList.filter(function(r){
                        return r.name.toLowerCase().includes(text);
                    })
                }
                initRoleTable(roleList);
                searchBox.value = '';
            })
            
            $('#btnNew').click(function(){
                var roleName = document.getElementById("txtroleName").value;
                $.ajax({
                    headers: {
                        "__RequestVerificationToken": antiForgeryToken
                    },
                    type: "POST",
                    //url: '@Url.Action("Create","role")',
                    url: '/role/create',
                    data: { Name: roleName },
                    success: function (res) {
                        console.log(res);
                        if (res.ret == 1) {
                            AddNewRoleToStore(res.msg);
                            AddNewRoleToTable(res.msg);
                            document.getElementById("txtroleName").value = '';
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
            })

            $('#btnSave').click(function(){
                var roleName = document.getElementById("txtroleName").value;
                console.log(roleName, selected.name);
                if(roleName === selected.name)
                    return;
                
                $.ajax({
                    headers: {
                        "__RequestVerificationToken": antiForgeryToken
                    },
                    type: "POST",
                    //url: '@Url.Action("Create","role")',
                    url: '/role/edit',
                    data: {roleid: selected.id, rolename: roleName},
                    success: function (res) {
                        console.log("Edit ok",res);
                        if (res.ret == 1) {
                            UpdateRoleToStore(res.msg);
                            UpdateRoleToTable(res.msg);
                        }
                        ShowMessage(res);
                    },
                    error: function (res) {
                        console.log("Edit role error", res);
                        res.ret = 0;
                        res.userMsg = "Unhandle exception error";
                        ShowMessage(res);
                    }
                })
            })

            $('#btnDel').click(function(){
                if(!selected)
                    return;
                $.ajax({
                    headers: {
                        "__RequestVerificationToken": antiForgeryToken
                    },
                    type: "POST",
                    //url: '@Url.Action("Create","role")',
                    url: '/role/delete/'+selected.id,
                    success: function (res) {
                        if (res.ret == 1) {
                            DeletRoleInTable(selected.id);
                            DeletRoleInStore(selected.id);
                            document.getElementById('txtroleName').value = '';
                            selected = null;
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
            })


            var ShowMessage = function (res) {
                var message = document.getElementById("StatusMessage");
                var errorsField = document.getElementById("errorModel");
                errorsField.innerHTML = '';
                
                message.innerHTML = res.userMsg;
                var className = "text-danger";
                if (res.ret == 1) {
                    className = "text-success";
                }
                else{
                    if(res.msg){
                        
                        res.msg.errors.forEach(e =>{
                            var p = document.createElement('p');
                            p.innerHTML = `${e.code}: ${e.description}`;
                            p.classList = className;
                            errorsField.appendChild(p);
                        })
                    }
                    
                }
                message.classList = className;
                setTimeout(function () {
                    console.log("timeout");
                    message.innerHTML = "";
                }, 30000);
            }

            var initRoleTable = function(roleList)
            { 
                document.getElementById("tableRole").innerHTML = '';
                roleList.forEach(role => AddNewRoleToTable(role));
            }

            var AddNewRoleToTable = function (role) {
                var roleTable = document.getElementById("tableRole");
                var roleNew = document.createElement("tr");
                roleNew.value = role;
                roleNew.id = role.id;

                //role Name data
                var roleName = document.createElement("td");
                roleName.innerHTML = role.name;
                roleNew.appendChild(roleName);

                // // delete button
                // var roleDel = document.createElement('td');
                // var roleDelBtn = document.createElement('a');
                // roleDelBtn.classList = "btn btn-danger btn-sm";
                // roleDelBtn.href = '/Role/Delete/' + role.id;
                // roleDelBtn.innerHTML = 'Xóa';
                // roleDel.appendChild(roleDelBtn);
                // roleNew.appendChild(roleDel);

                roleTable.appendChild(roleNew);
                roleNew.addEventListener('click', function (e) {
                    var txtroleName = document.getElementById("txtroleName");
                    selected = this.value;
                    txtroleName.value = selected.name;
                })
            }
            var UpdateRoleToTable = function(role){
                var roletr = document.getElementById(role.id);
                roletr.value = role;
                selected = role;
                roletr.firstElementChild.innerHTML = role.name;
            }

            var DeletRoleInTable = function(roleid){
                var roleTable = document.getElementById("tableRole");
                var roleDel = document.getElementById(roleid);
                roleTable.removeChild(roleDel);
            }; 

            var GetRoleStore = function(){
                return this.roles;
            };

            var SetRoleStore = function(newStore){
                this.roles = newStore;
            }

            var AddNewRoleToStore = function(role){
                this.roles.push(role);
                console.log(this.roles);
            };

            var DeletRoleInStore = function(roleID){
                var removeItem = this.roles.find(function(role){
                    return role.id == roleID;
                });

                if(removeItem){
                    this.roles.splice(this.roles.indexOf(removeItem), 1);
                }
                console.log(this.roles);
            };

            var UpdateRoleToStore = function(newRole)
            { 
                var modifyItem = this.roles.find(function(r){
                    return r.id === newRole.id
                });
                console.log(modifyItem);
                modifyItem.name = newRole.name;
                console.log(this.roles);
            }
        }
    };
}();
