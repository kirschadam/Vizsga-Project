import React from 'react';
import { useNavigate} from "react-router-dom";
import {Base_storage, User} from '../Variables';
import { BaseModal } from './BaseModal';

export function CreateWindow({onClose}) {
	const navigate = useNavigate();

    function Create(e)
    {
        e.preventDefault();
        fetch(Base_storage + "AddStock",
        { 
            method: "POST",
            headers:{'Content-Type': 'application/json'},
            body: JSON.stringify({
                uid: User.Uid,
                product: e.target.elements.product.value,
                location: e.target.elements.location.value
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
		<BaseModal Submitted={Create} onClosed={onClose}  title="Create Stock">
			<div className="from-group row pb-3">
                <label className="col-sm-3 col-form-label">Product:</label>
                <div className="col-sm-9">
                    <input type="text" name="product" className="form-control" />
                </div>
            </div>
			<div className="form-group row pb-3">
                <label className="col-sm-3 col-form-label">Location:</label>
                <div className="col-sm-9">
                    <input type="text" name="location" className="form-control" min="0" max="10000000" />
                </div>
            </div>
		add stock?
        </BaseModal>
    )
}

export function UpdateWindow({onClose, stock}){
	const navigate = useNavigate();

    function Update(e){
        e.preventDefault();
		fetch(Base_storage + "UpdateStock",
		{ 
            method: "PUT",
			headers:{'Content-Type': 'application/json'},
			body: JSON.stringify({
				uid: User.Uid,
				id: stock.Id,
				product: e.target.elements.product.value,
				quantity: e.target.elements.quantity.value,
				location: e.target.elements.location.value
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
		<BaseModal Submitted={Update} onClosed={onClose} title="Update Stock">
			<div className="from-group row pb-3">
                <label className="col-sm-3 col-form-label">Product:</label>
                <div className="col-sm-9">
                    <input type="text" name="product" className="form-control" defaultValue={stock == null ? "" : stock.Product} />
                </div>
            </div>
			<div className="form-group row pb-3">
                <label className="col-sm-3 col-form-label">Quantity:</label>
                <div className="col-sm-9">
                    <input type="number" name="quantity" className="form-control" defaultValue={stock == null ? "" : stock.Quantity}/>
                </div>
            </div>
			<div className="form-group row pb-3">
                <label className="col-sm-3 col-form-label">Location:</label>
                <div className="col-sm-9">
                    <input type="text" name="location" className="form-control" defaultValue={stock == null ? "" : stock.Location} />
                </div>
            </div>	
		update stock?
		</BaseModal>
    )
}

export function DeleteWindow({onClose, stock}) {
	const navigate = useNavigate();

    function Delete(e){
        e.preventDefault();
        fetch(Base_storage + "DeleteStock",
        { 
            method: "DELETE",
            headers:{'Content-Type': 'application/json'},
            body: JSON.stringify({
                uid: User.Uid,
                id: stock.Id
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
        <BaseModal Submitted={Delete} onClosed={onClose}  title="Delete Stock">
            Are you sure to delete the stock of <span><b>{stock == null ? "" : stock.Product}</b></span>
            <br/>at location <span><b>{stock == null ? "" : stock.Location}</b></span> ?
        </BaseModal>
    )
}