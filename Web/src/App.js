import './App.css';
import Products from './List_Pages/ProductList';
import Stocks from './List_Pages/StockList';
import Sales from './List_Pages/SaleList';
import Purchases from './List_Pages/PurchaseList';
import Locations from './List_Pages/LocationList';
import Regions from './List_Pages/RegionList';
import Users from './List_Pages/UserList';
import { BrowserRouter, Route, Routes, Navigate, Link} from "react-router-dom";
import 'bootstrap/dist/css/bootstrap.min.css';
import { Login, Logout } from './Login_Logout';
import useLocalStorage from 'use-local-storage';
import 'react-slideshow-image/dist/styles.css';

function App() {
  const defaultDark = window.matchMedia('(prefers-color-scheme: dark)').matches;
  const [theme, setTheme] = useLocalStorage('theme', defaultDark ? 'dark' : 'light');

  const switchTheme = () =>{
    const newTheme = theme === 'light' ? 'dark' : 'light';
    setTheme(newTheme);
  }

  function currentSet(e){
    var id = sessionStorage.getItem("id");

    if(id != null && id !== ""){
      document.getElementById(id).classList.remove('res-current');
    }

    e.target.classList.add('res-current');

    sessionStorage.setItem("id", e.target.id);
  }

  return (
    <div className="App res-primary" data-theme={theme}>
      <div>
        <button onClick={switchTheme} id="themeButton" className="res-primary res-background">{theme === 'light' ? 'Dark' : 'Light'}</button>
      </div>
      <BrowserRouter>
        <div>
          <nav className="res-background float-start" id="navbar">
            <div className='col-1'>
              <Link className='res-primary res-color-hover text-decoration-none' onClick={currentSet} id="product" to="/products"> Products </Link>
            </div>
            <div className='col-1'>
              <Link className='res-primary res-color-hover text-decoration-none' onClick={currentSet} id="stock" to="/stocks"> Stocks </Link>
            </div>
            <div className='col-1'>
              <Link className='res-primary res-color-hover text-decoration-none' onClick={currentSet} id="sale" to="/sales"> Sales </Link>
            </div>
            <div className='col-1'>
              <Link className='res-primary res-color-hover text-decoration-none' onClick={currentSet} id="purchase" to="/purchases"> Purchases </Link>
            </div>
            <div className='col-1'>
              <Link className='res-primary res-color-hover text-decoration-none' onClick={currentSet} id="location" to="/locations"> Locations </Link>
            </div>
            <div className='col-1'>
              <Link className='res-primary res-color-hover text-decoration-none' onClick={currentSet} id="region" to="/regions"> Regions </Link>
            </div>
            <div className='col-1'>
              <Link className='res-primary res-color-hover text-decoration-none' onClick={currentSet} id="user" to="/users"> Users </Link>
            </div>
          </nav>
        </div>
        <div id="logout">
          <Logout/>
        </div>
        <Routes>
          <Route path="/login" exact element={<Login/>}></Route>
          <Route path="/products" exact element={<Products/>}></Route>
          <Route path="/stocks" exact element={<Stocks/>}></Route>
          <Route path="/sales" exact element={<Sales/>}></Route>
          <Route path="/purchases" exact element={<Purchases/>}></Route>
          <Route path="/locations" exact element={<Locations/>}></Route>
          <Route path="/regions" exact element={<Regions/>}></Route>
          <Route path="/Users" exact element={<Users/>}></Route>
          <Route
            path="*"
            element={<Navigate to="/login" />}
          />
        </Routes>
      </BrowserRouter>
    </div>
  );
}

export default App;
