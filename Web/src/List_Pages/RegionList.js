import React, {useState, useEffect} from 'react';
import { useNavigate} from "react-router-dom";
import {Base_location, User} from '../Variables';

function Regions() {
	const [regions, setRegions] = useState([]);
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

    	fetch(Base_location + "listregion?uid=" + User.Uid
		,{
      		method:"GET"
    	})
    	.then(res => res.json())
    	.then((response) =>{
      		console.log(response.Message);
      		setRegions(response.Regions);
    	})
    	.catch((response) => {
      		console.log(response.Message);
    	});
  	}, [navigate]);

  	function SearchSubmit(e){
    	e.preventDefault();
    	e = e.target.elements;
    	fetch(Base_location + "listregion?uid=" + User.Uid + "&region=" + e.region.value + "&limit=" + e.limit.value
    	,{
      		method:"GET"
    	})
    	.then(res => res.json())
    	.then((response) =>{
      		console.log(response.Message);
      		setRegions(response.Regions);
    	})
    	.catch((response) => {
      		console.log(response.Message);
    	});
  	}

	function locationList(locations){
		var string = "";
		var i = 0;
		locations.forEach(element => {
		  if(i !== 0){
			  string += ", " + element.Name
		  }
		  else string += element.Name
		})

		return string;
	}

  	return (
    	<div className='res-primary res-background'>
      		<ul className="list-group w-100">
        		<div className="row border-bottom py-2 w-100">
          			<h5 className="col-xs-12 col-sm-1">Id</h5>
          			<h5 className="col-xs-4 col-sm-3">region</h5>
          			<h5 className="col-xs-8 col-sm-6">Locations</h5>
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
              				<label className='form-label mx-2' htmlFor="region">Name:</label>
              				<input type="text" name="region"/>
            			</div>
						<div className="col-xs-8 col-sm-6"> </div>
            			<div className='col-xs-12 col-sm-2'>
              				<label className='form-label mx-2' htmlFor="limit">Limit:</label>
              				<input type="number" name="limit" className='form-text w-50'></input>
            			</div>
         	 		</div>
        		</ul>
      		</form>
			<div className="list res-background">
      			{regions.map((region) => (
        			<div key={region.Id} className="row border-bottom  py-2 w-100">
          				<div className="col-xs-12 col-sm-1">{region.Id}</div>
          				<div className="col-xs-4 col-sm-3">{region.Name}</div>
          				<div className="col-xs-8 col-sm-6">{locationList(region.Locations)}</div>
        			</div>
      			))}
			</div>
    	</div>
  	)
}

export default Regions