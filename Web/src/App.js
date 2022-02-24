import './App.css';
import Test from './Test';
import { BrowserRouter, Route, Routes, Navigate} from "react-router-dom";
import 'bootstrap/dist/css/bootstrap.min.css';
import { Bejelentkezes } from './Bejelentkezes';

function App() {
  return (
    <div className="App">
      <BrowserRouter>
        <Routes>
          <Route path="/bejelentkezes" exact element={<Bejelentkezes/>}></Route>
          <Route path="/test" exact element={<Test/>}></Route>
          <Route
            path="*"
            element={<Navigate to="/bejelentkezes" />}
          />
        </Routes>
    </BrowserRouter>
    </div>
  );
}

export default App;
