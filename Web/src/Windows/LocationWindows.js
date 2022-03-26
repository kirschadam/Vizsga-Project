import { Base_location, User } from '../Variables';
import { BaseModal } from './BaseModal';

export function CreateWindow({onClose}) {

    function Create(e){
        e.preventDefault();
        e = e.target.elements;
        fetch(Base_location + "AddLocation",
        { 
            method: "POST",
            headers:{'Content-Type': 'application/json'},
            body: JSON.stringify({
                uid: User.Uid,
                location: e.location.value,
                region: e.region.value
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
		<BaseModal Submitted={Create} onClosed={onClose} title="Create Location">

			<div className="from-group row pb-3">
                <label className="col-sm-3 col-form-label">Location:</label>
                <div className="col-sm-9">
                    <input type="text" name="location" className="form-control" />
                </div>
            </div>

			<div className="form-group row pb-3">
                <label className="col-sm-3 col-form-label">Region:</label>
                <div className="col-sm-9">
                    <input type="text" name="region" className="form-control" min="0" max="10000000" />
                </div>
            </div>

		    add Location?
        </BaseModal>
    )
}

export function UpdateWindow({onClose, location}){

    function Update(e){
        e.preventDefault();
        e = e.target.elements;
		fetch(Base_location + "UpdateLocation",
		{ 
            method: "PUT",
			headers:{'Content-Type': 'application/json'},
			body: JSON.stringify({
				uid: User.Uid,
				id: location.Id,
				location: e.location.value,
				region: e.region.value
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
		<BaseModal Submitted={Update} onClosed={onClose} title="Update Location">

			<div className="from-group row pb-3">
                <label className="col-sm-3 col-form-label">Name:</label>
                <div className="col-sm-9">
                    <input type="text" name="location" className="form-control" defaultValue={location == null ? "" : location.Name} />
                </div>
            </div>

			<div className="form-group row pb-3">
                <label className="col-sm-3 col-form-label">Region:</label>
                <div className="col-sm-9">
                    <input type="text" name="region" className="form-control" defaultValue={location == null ? "" : location.Region} />
                </div>
            </div>

		    update location?
		</BaseModal>
    )
}

export function DeleteWindow({onClose, location}) {

    function Delete(e){
        e.preventDefault();
        fetch(Base_location + "DeleteLocation",
        { 
            method: "DELETE",
            headers:{'Content-Type': 'application/json'},
            body: JSON.stringify({
                uid: User.Uid,
                id: location.Id
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
        <BaseModal Submitted={Delete} onClosed={onClose} title="Delete Location">
            Are you sure to delete the location <b>{location == null ? "" : location.Name}</b>?
        </BaseModal>
    )
}