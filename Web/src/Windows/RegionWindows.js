import React from 'react';
import { useNavigate} from "react-router-dom";
import {Base_location, User} from '../Variables';
import { BaseModal } from './BaseModal';

export function CreateWindow({onClose}) {
	const navigate = useNavigate();

    function Create(e)
    {
        e.preventDefault();
        fetch(Base_location + "AddRegion",
        { 
            method: "POST",
            headers:{'Content-Type': 'application/json'},
            body: JSON.stringify({
                uid: User.Uid,
                region: e.target.elements.region.value
            })
        })
        .then(res => res.json())
        .then((response) =>{
            alert(response);
            if(response === "X"){
                alert("This is a specifice alert!");
                return;
            }
            else{
                navigate("/");
            }
        })
        .catch((response) => {
            console.log(response);
            return;
        });
    }

    return (
		<BaseModal Submitted={Create} onClosed={onClose}  title="Create Region">
			<div className="from-group row pb-3">
                <label className="col-sm-3 col-form-label">Region:</label>
                <div className="col-sm-9">
                    <input type="text" name="region" className="form-control" />
                </div>
            </div>
		add region?
        </BaseModal>
    )
}

export function UpdateWindow({onClose, region}){
	const navigate = useNavigate();

    function Update(e){
        e.preventDefault();
		fetch(Base_location + "UpdateRegion",
		{ 
            method: "PUT",
			headers:{'Content-Type': 'application/json'},
			body: JSON.stringify({
				uid: User.Uid,
				id: region.Id,
				region: e.target.elements.region.value
		    })
		})
		.then(res => res.json())
		.then((response) =>{
			alert(response);
			if(response === "X"){
				alert("This is a specifice alert!");
				return;
			}
            else{
                navigate("/");
            }
		})
		.catch((response) => {
			console.log(response);
			return;
		});
    }

    return(
		<BaseModal Submitted={Update} onClosed={onClose} title="Update Region">
			<div className="from-group row pb-3">
                <label className="col-sm-3 col-form-label">Region:</label>
                <div className="col-sm-9">
                    <input type="text" name="region" className="form-control" defaultValue={region == null ? "" : region.Name} />
                </div>
            </div>
		update region?
		</BaseModal>
    )
}

export function DeleteWindow({onClose, region}) {
	const navigate = useNavigate();

    function Delete(e){
        e.preventDefault();
        fetch(Base_location + "DeleteRegion",
        { 
            method: "DELETE",
            headers:{'Content-Type': 'application/json'},
            body: JSON.stringify({
                uid: User.Uid,
                id: region.Id
            })
        })
        .then(res => res.json())
        .then((response) =>{
            alert(response);
            if(response === "X"){
                alert("This is a specifice alert!");
                return;
            }
            else{
                navigate("/");
            }
        })
        .catch((response) => {
            console.log(response);
            return;
        });
    }

    return(
        <BaseModal Submitted={Delete} onClosed={onClose}  title="Delete Region">
            Are you sure to delete the region <span><b>{region == null ? "" : region.Name}</b></span> ?
        </BaseModal>
    )
}