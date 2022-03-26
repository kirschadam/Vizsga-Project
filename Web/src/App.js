import './App.css';
import Products from './List_Pages/ProductList';
import Stocks from './List_Pages/StockList';
import Sales from './List_Pages/SaleList';
import Purchases from './List_Pages/PurchaseList';
import Locations from './List_Pages/LocationList';
import Regions from './List_Pages/RegionList';
import Users from './List_Pages/UserList';
import { Login, Logout } from './Login_Logout';
import {User} from './Variables'
import useLocalStorage from 'use-local-storage';
import 'bootstrap/dist/css/bootstrap.min.css';
import { BrowserRouter, Route, Routes, Navigate, Link } from "react-router-dom";
import { useState, useEffect } from 'react';

function App() {
	//Constants used to set the default/dark themes, the choice of theme is stored locally
	const defaultDark = window.matchMedia('(prefers-color-scheme: dark)').matches;
	const [theme, setTheme] = useLocalStorage('theme', defaultDark ? 'dark' : 'light');
	const switchTheme = () =>{
	  const newTheme = theme === 'light' ? 'dark' : 'light';
	  setTheme(newTheme);
	}

	//Constants used to set which navlink should be different color (meaning where we are), and setting the text of who is currently logged in
	const [headtext, setHeadText] = useState("");
	const [currentPage, setCurrentPage] = useState("");

	//On load and change of currentPage, reload elements
  	useEffect(() => {
		if(User.Uid !== null && User.Uid !== ""){
			setHeadText("Logged in user: " + User.Details.Username + " (" + User.Details.Permission + ")")
		}
		else setHeadText("");

		var id = sessionStorage.getItem("id");

		if(id != null && id !== ""){
      		document.getElementById(id).classList.remove('res-current');
		}

		if(currentPage != null && currentPage !== ""){
			document.getElementById(currentPage).classList.add('res-current');

			//Save current navlink id so we can remove class when we switch pages
			sessionStorage.setItem("id", currentPage);
		}
  	}, [currentPage])


  	return (
    	<div className="App res-primary res-background" data-theme={theme}>

      		<div>
        		<button onClick={switchTheme} id="themeButton" className="res-primary res-background">{theme === 'light' ? 'Dark' : 'Light'}</button>
      		</div>

      		<BrowserRouter>
          			<nav className="res-background float-start" id="navbar">

            			<div className='col-1'>
              				<Link className='res-primary res-color-hover text-decoration-none' id="product" to="/products"> Products </Link>
            			</div>
            			<div className='col-1'>
              				<Link className='res-primary res-color-hover text-decoration-none' id="stock" to="/stocks"> Stocks </Link>
            			</div>
            			<div className='col-1'>
              				<Link className='res-primary res-color-hover text-decoration-none' id="sale" to="/sales"> Sales </Link>
						</div>
						<div className='col-1'>
							<Link className='res-primary res-color-hover text-decoration-none' id="purchase" to="/purchases"> Purchases </Link>
						</div>
						<div className={'col-1 ' + (User.Details.Permission > 8 ? "" : "invisible")}>
							<Link className='res-primary res-color-hover text-decoration-none' id="location" to="/locations"> Locations </Link>
						</div>
						<div className={'col-1 ' + (User.Details.Permission > 8 ? "" : "invisible")}>
							<Link className='res-primary res-color-hover text-decoration-none' id="region" to="/regions"> Regions </Link>
						</div>
						<div className={'col-1 ' + (User.Details.Permission > 8 ? "" : "invisible")}>
							<Link className='res-primary res-color-hover text-decoration-none' id="user" to="/users"> Users </Link>
						</div>

          			</nav>


        		<div>
          			{headtext}
        		</div>


        		<div id="logout">
          			<Logout headtext={setHeadText}/>
        		</div>
				
        		<Routes>

					<Route path="/login" exact element={<Login headtext={setHeadText}/>}></Route>
					<Route path="/products" exact element={<Products currentpage={setCurrentPage}/>}></Route>
					<Route path="/stocks" exact element={<Stocks currentpage={setCurrentPage}/>}></Route>
					<Route path="/sales" exact element={<Sales currentpage={setCurrentPage}/>}></Route>
					<Route path="/purchases" exact element={<Purchases currentpage={setCurrentPage}/>}></Route>
					<Route path="/locations" exact element={<Locations currentpage={setCurrentPage}/>}></Route>
					<Route path="/regions" exact element={<Regions currentpage={setCurrentPage}/>}></Route>
					<Route path="/Users" exact element={<Users currentpage={setCurrentPage}/>}></Route>
					<Route
						path="*"
						element={<Navigate to={"/login"}/>}
					/>

        		</Routes>
      		</BrowserRouter>
    	</div>
  	);
}

export default App;
