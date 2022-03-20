import React from 'react';
import {Base_user, User} from '../Variables';
import { BaseModal } from './BaseModal';
import CryptoJS from 'react-native-crypto-js';

export function CreateWindow({onClose}) {

    function Create(e)
    {
        e.preventDefault();
        fetch(Base_user + "RegisterUser",
        { 
            method: "POST",
            headers:{'Content-Type': 'application/json'},
            body: JSON.stringify({
                uid: User.Uid,
                username: e.target.elements.username.value,
                password: String(CryptoJS.MD5(e.target.elements.password.value)),
                location: e.target.elements.location.value,
                permission: e.target.elements.permission.value
            })
        })
        .then(res => res.json())
        .then((response) =>{
            alert(response);
            if(response === "X"){
                alert("This is a specifice alert!");
                return;
            }
        })
        .catch((response) => {
            console.log(response);
            return;
        });
        
        window.location.reload();
    }

    return (
		<BaseModal Submitted={Create} onClosed={onClose}  title="Create User">
			<div className="from-group row pb-3">
                <label className="col-sm-3 col-form-label">Username:</label>
                <div className="col-sm-9">
                    <input type="text" name="username" className="form-control" />
                </div>
            </div>
			<div className="form-group row pb-3">
                <label className="col-sm-3 col-form-label">Password:</label>
                <div className="col-sm-9">
                    <input type="text" name="password" className="form-control" min="0" max="10000000" />
                </div>
            </div>	
			<div className="form-group row pb-3">
                <label className="col-sm-3 col-form-label">Location:</label>
                <div className="col-sm-9">
                    <input type="text" name="location" className="form-control" min="0" max="10000000" />
                </div>
            </div>
			<div className="form-group row pb-3">
                <label className="col-sm-3 col-form-label">Permission:</label>
                <div className="col-sm-9">
                    <input type="number" name="permission" className="form-control" min="0" max="10000000" />
                </div>
            </div>
		add user?
        </BaseModal>
    )
}

export function UpdateWindow({onClose, user}){

    function Update(e){
        e.preventDefault();
		fetch(Base_user + "UpdateUser",
		{ 
            method: "PUT",
			headers:{'Content-Type': 'application/json'},
			body: JSON.stringify({
				uid: User.Uid,
				id: user.Id,
				username: e.target.elements.username.value,
				password: String(CryptoJS.MD5(e.target.elements.newpassword.value)),
				location: e.target.elements.location.value,
				permission: e.target.elements.permission.value,
				active: e.target.elements.active.value
		    })
		})
		.then(res => res.json())
		.then((response) =>{
			alert(response);
			if(response === "X"){
				alert("This is a specifice alert!");
				return;
			}
		})
		.catch((response) => {
			console.log(response);
			return;
		});
		window.location.reload();
    }

    return(
		<BaseModal Submitted={Update} onClosed={onClose} title="Update User">
			<div className="from-group row pb-3">
                <label className="col-sm-3 col-form-label">Username:</label>
                <div className="col-sm-9">
                    <input type="text" name="username" className="form-control" defaultValue={user == null ? "" : user.Username} />
                </div>
            </div>	
			<div className={"form-group row pb-3 " + (User.Details.Permission > 7 ? "visible" : "invisible")}>
                <label className={"col-sm-3 col-form-label " + (User.Details.Permission > 7 ? "visible" : "invisible")}>New Password:</label>
                <div className={"col-sm-9 " + (User.Details.Permission > 7 ? "visible" : "invisible")}>
                    <input type="text" name="newpassword" className="form-control" defaultValue=""/>
                </div>
            </div>
			<div className="form-group row pb-3">
                <label className="col-sm-3 col-form-label">Location:</label>
                <div className="col-sm-9">
                    <input type="text" name="location" className="form-control" defaultValue={user == null ? "" : user.Location}/>
                </div>
            </div>	
			<div className={"form-group row pb-3 " + (User.Details.Permission > 8 ? "visible" : "invisible")}>
                <label className={"col-sm-3 col-form-label " + (User.Details.Permission > 8 ? "visible" : "invisible")}>Permission:</label>
                <div className={"col-sm-9 " + (User.Details.Permission > 8 ? "visible" : "invisible")}>
                    <input type="number" name="permission" className="form-control" defaultValue={user == null ? "" : user.Permission}/>
                </div>
            </div>
			<div className={"form-group row pb-3 " + (User.Details.Permission > 8 ? "visible" : "invisible")}>
                <label className={"col-sm-3 col-form-label " + (User.Details.Permission > 8 ? "visible" : "invisible")}>Active:</label>
                <div className={"col-sm-9 " + (User.Details.Permission > 8 ? "visible" : "invisible")}>
                    <input type="number" name="active" className="form-control" defaultValue={user == null ? "" : user.Active}/>
                </div>
            </div>
		update user?
		</BaseModal>
    )
}

export function DeleteWindow({onClose, user}) {

    function Delete(e){
        e.preventDefault();
        fetch(Base_user + "DeleteUser",
        { 
            method: "DELETE",
            headers:{'Content-Type': 'application/json'},
            body: JSON.stringify({
                uid: User.Uid,
                id: user.Id
            })
        })
        .then(res => res.json())
        .then((response) =>{
            alert(response);
            if(response === "X"){
                alert("This is a specifice alert!");
                return;
            }
        })
        .catch((response) => {
            console.log(response);
            return;
        });
        window.location.reload();
    }

    return(
        <BaseModal Submitted={Delete} onClosed={onClose}  title="Delete User">
            Are you sure to delete the user <span><b>{user == null ? "" : user.Username}</b></span> ?
        </BaseModal>
    )
}