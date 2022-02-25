import './App.css';
import Stocks from './StockList';
import { BrowserRouter, Route, Routes, Navigate} from "react-router-dom";
import 'bootstrap/dist/css/bootstrap.min.css';
import { Login } from './Login_Logout';
import useLocalStorage from 'use-local-storage'

function App() {
  const defaultDark = window.matchMedia('(prefers-color-scheme: dark)').matches;
  const [theme, setTheme] = useLocalStorage('theme', defaultDark ? 'dark' : 'light');

  const switchTheme = () =>{
    const newTheme = theme === 'light' ? 'dark' : 'light';
    setTheme(newTheme);
  }

  return (
    <div className="App res-primary res-background" data-theme={theme}>
      <div>
        <button onClick={switchTheme} id="themeButton" className="res-primary res-background">{theme === 'light' ? 'Light' : 'Dark'}</button>
      </div>
      <BrowserRouter>
        <Routes>
          <Route path="/login" exact element={<Login/>}></Route>
          <Route path="/stocks" exact element={<Stocks/>}></Route>
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
