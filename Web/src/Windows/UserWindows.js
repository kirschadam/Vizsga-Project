import { Base_user, User } from '../Variables';
import { BaseModal } from './BaseModal';
import CryptoJS from 'react-native-crypto-js';

export function CreateWindow({onClose}) {

    function Create(e){
        e.preventDefault();
        e = e.target.elements;
        fetch(Base_user + "RegisterUser",
        { 
            method: "POST",
            headers:{'Content-Type': 'application/json'},
            body: JSON.stringify({
                uid: User.Uid,
                username: e.username.value,
                password: String(CryptoJS.MD5(e.password.value)),
                location: e.location.value,
                permission: e.permission.value
            })
        })
        .then(res => res.json())
        .then((res) =>{
            alert(res);
            window.location.reload();
        })
        .catch((res) => {
            console.log(res);
        });
    }

    return(
		<BaseModal Submitted={Create} onClosed={onClose} title="Create User">

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
        e = e.target.elements;
		fetch(Base_user + "UpdateUser",
		{ 
            method: "PUT",
			headers:{'Content-Type': 'application/json'},
			body: JSON.stringify({
				uid: User.Uid,
				id: user.Id,
				username: e.username.value,
				password: String(CryptoJS.MD5(e.newpassword.value)),
				location: e.location.value,
				permission: e.permission.value,
				active: e.active.value
		    })
		})
		.then(res => res.json())
		.then((res) =>{
			alert(res);
			window.location.reload();
		})
		.catch((res) => {
			console.log(res);
		});
    }

    return(
		<BaseModal Submitted={Update} onClosed={onClose} title="Update User">

			<div className="from-group row pb-3">
                <label className="col-sm-3 col-form-label">Username:</label>
                <div className="col-sm-9">
                    <input type="text" name="username" className="form-control" defaultValue={user == null ? "" : user.Username} />
                </div>
            </div>	

			<div className="form-group row pb-3">
                <label className="col-sm-3 col-form-label">New Password:</label>
                <div className="col-sm-9">
                    <input type="text" name="newpassword" className="form-control" defaultValue=""/>
                </div>
            </div>

			<div className="form-group row pb-3">
                <label className="col-sm-3 col-form-label">Location:</label>
                <div className="col-sm-9">
                    <input type="text" name="location" className="form-control" defaultValue={user == null ? "" : user.Location}/>
                </div>
            </div>	

			<div className="form-group row pb-3">
                <label className="col-sm-3 col-form-label">Permission:</label>
                <div className="col-sm-9">
                    <input type="number" name="permission" className="form-control" defaultValue={user == null ? "" : user.Permission}/>
                </div>
            </div>

			<div className="form-group row pb-3">
                <label className="col-sm-3 col-form-label">Active:</label>
                <div className="col-sm-9">
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
        .then((res) =>{
            alert(res);
            window.location.reload();
        })
        .catch((res) => {
            console.log(res);
        });
    }

    return(
        <BaseModal Submitted={Delete} onClosed={onClose} title="Delete User">
            Are you sure to delete the user <b>{user == null ? "" : user.Username}</b>?
        </BaseModal>
    )
}