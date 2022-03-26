import { Base_storage, User } from '../Variables';
import { BaseModal } from './BaseModal';

export function CreateWindow({onClose}) {

    function Create(e){
        e.preventDefault();
        e = e.target.elements;
        fetch(Base_storage + "AddStock",
        { 
            method: "POST",
            headers:{'Content-Type': 'application/json'},
            body: JSON.stringify({
                uid: User.Uid,
                product: e.product.value,
                location: e.location.value
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
		<BaseModal Submitted={Create} onClosed={onClose} title="Create Stock">

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

    function Update(e){
        e.preventDefault();
        e = e.target.elements;
		fetch(Base_storage + "UpdateStock",
		{ 
            method: "PUT",
			headers:{'Content-Type': 'application/json'},
			body: JSON.stringify({
				uid: User.Uid,
				id: stock.Id,
				product: e.product.value,
				quantity: e.quantity.value,
				location: e.location.value
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
        .then((res) =>{
            alert(res);
            window.location.reload();
        })
        .catch((res) => {
            console.log(res);
        });
    }

    return(
        <BaseModal Submitted={Delete} onClosed={onClose} title="Delete Stock">
            Are you sure to delete the stock of <b>{stock == null ? "" : stock.Product}</b><br/>
            at location <b>{stock == null ? "" : stock.Location}</b>?
        </BaseModal>
    )
}