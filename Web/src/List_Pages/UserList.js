import React, {useState, useEffect} from 'react';
import { useNavigate} from "react-router-dom";
import {Base_user, User} from '../Variables';

function Users() {
  const [users, setUsers] = useState([]);
	const navigate = useNavigate();

	useEffect(() =>{
		if(User.Uid === null || User.Uid === "")
		{ 
			User.Uid = sessionStorage.getItem("Uid");
			User.Details = JSON.parse(sessionStorage.getItem("Details"));
			if(User.Uid === null || User.Uid === ""){
			console.log("User data could not be loaded from cache, redirecting!"); 
			navigate("/login");
      		}
      		else {console.log("User data loaded from cache!");}
    	}

    	fetch(Base_user + "listuser?uid=" + User.Uid
		,{
      		method:"GET"
    	})
    	.then(res => res.json())
    	.then((response) =>{
      		console.log(response.Message);
      		setUsers(response.Users);
    	})
    	.catch((response) => {
      		console.log(response.Message);
    	});
  	}, [navigate]);

  	function SearchSubmit(e){
    	e.preventDefault();
    	e = e.target.elements;
    	fetch(Base_user + "listuser?uid=" + User.Uid + "&username=" + e.username.value + "&location=" + e.location.value
      		+ "&permissionover=" + e.over.value + "&permissionunder=" + e.under.value + "&active=" + e.active.value + "&limit=" + e.limit.value
    	,{
      		method:"GET"
    	})
    	.then(res => res.json())
    	.then((response) =>{
      		console.log(response.Message);
      		setUsers(response.Users);
    	})
    	.catch((response) => {
      		console.log(response.Message);
    	});
  	}

  	return (
    	<div className='res-primary'>
      		<ul className="list-group w-100">
        		<div className="row border-bottom py-2 w-100">
          			<h5 className="col-xs-12 col-sm-1">Id</h5>
          			<h5 className="col-xs-4 col-sm-3">Username</h5>
          			<h5 className="col-xs-8 col-sm-3">Location</h5>
          			<h5 className="col-xs-10 col-sm-3">Permission</h5>
          			<h5 className="col-xs-10 col-sm-1">Status</h5>
        		</div>
      		</ul>
      		<form onSubmit={SearchSubmit} id="filterForm">
        		<ul className="list-group w-100">
          			<div className="row border-bottom py-2 w-100">
            			<div className='col-xs-12 col-sm-1'>
              				<button type="submit" className='btn btn-dark'>Filter</button>
              				<button type="Reset" className='btn btn-dark'>Reset</button>
            			</div>
            			<div className="col-xs-8 col-sm-3">
              				<label className='form-label mx-2' htmlFor="username">Name:</label>
              				<input type="text" name="username"/>
            			</div>
            			<div className="col-xs-8 col-sm-3">
              				<label className='form-label mx-2' htmlFor="location">Name:</label>
              				<input type="text" name="location"/>
            			</div>
            			<div className="col-xs-4 col-sm-3">
              				<div className=' float-start'>
                				<label className='form-label me-1' htmlFor="over">Over:</label>
                				<input type="number" name="over" className='form-text w-25'/>
                				<label className='form-label ms-2 me-1' htmlFor="under">Under:</label>
                				<input type="number" name="under" className='form-text w-25'/>
              				</div>
            			</div>
            			<div className="col-xs-8 col-sm-1">
              				<label className='form-label mx-2' htmlFor="active">Active?(1/0)</label>
              				<input type="text" name="active" className='w-50'/>
            			</div>
            			<div className='col-xs-12 col-sm-1'>
              				<label className='form-label mx-2' htmlFor="limit">Limit:</label>
              				<input type="number" name="limit" className='form-text w-50'></input>
            			</div>
         	 		</div>
        		</ul>
      		</form>
			<div className="list">
      			{users.map((user) => (
        			<div key={user.Id} className="row border-bottom  py-2 w-100">
          				<div className="col-xs-12 col-sm-1">{user.Id}</div>
          				<div className="col-xs-4 col-sm-3">{user.Username}</div>
          				<div className="col-xs-8 col-sm-3">{user.Location}</div>
          				<div className="col-xs-10 col-sm-3">{user.Permission}</div>
          				<div className="col-xs-10 col-sm-1">{user.Active === 1 ? "Active" : "Inactive"}</div>
        			</div>
      			))}
			</div>
    	</div>
  	)
}

export default Users