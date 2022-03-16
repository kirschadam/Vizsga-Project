import React, {useState, useEffect} from 'react';
import { useNavigate} from "react-router-dom";
import {Base_location, User} from '../Variables';

function Locations() {
	const [locations, setLocations] = useState([]);
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

    	fetch(Base_location + "listlocation?uid=" + User.Uid
		,{
      		method:"GET"
    	})
    	.then(res => res.json())
    	.then((response) =>{
      		console.log(response.Message);
      		setLocations(response.Locations);
    	})
    	.catch((response) => {
      		console.log(response.Message);
    	});
  	}, [navigate]);

  	function SearchSubmit(e){
    	e.preventDefault();
    	e = e.target.elements;
    	fetch(Base_location + "listlocation?uid=" + User.Uid + "&location=" + e.location.value + "&region=" + e.region.value + "&limit=" + e.limit.value
    	,{
      		method:"GET"
    	})
    	.then(res => res.json())
    	.then((response) =>{
      		console.log(response.Message);
      		setLocations(response.Locations);
    	})
    	.catch((response) => {
      		console.log(response.Message);
    	});
  	}

  	return (
    	<div className='res-primary res-background'>
      		<ul className="list-group w-100">
        		<div className="row border-bottom py-2 w-100">
          			<h5 className="col-xs-12 col-sm-1">Id</h5>
          			<h5 className="col-xs-4 col-sm-3">Location</h5>
          			<h5 className="col-xs-8 col-sm-3">Region</h5>
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
              				<label className='form-label mx-2' htmlFor="location">Name:</label>
              				<input type="text" name="location"/>
            			</div>
						<div className="col-xs-8 col-sm-3">
              				<label className='form-label mx-2' htmlFor="region">Name:</label>
              				<input type="text" name="region"/>
            			</div>
            			<div className='col-xs-12 col-sm-2'>
              				<label className='form-label mx-2' htmlFor="limit">Limit:</label>
              				<input type="number" name="limit" className='form-text w-50'></input>
            			</div>
         	 		</div>
        		</ul>
      		</form>
			<div className="list res-background">
      			{locations.map((location) => (
        			<div key={location.Id} className="row border-bottom  py-2 w-100">
          				<div className="col-xs-12 col-sm-1">{location.Id}</div>
          				<div className="col-xs-4 col-sm-3">{location.Name}</div>
          				<div className="col-xs-8 col-sm-3">{location.Region}</div>
        			</div>
      			))}
			</div>
    	</div>
  	)
}

export default Locations