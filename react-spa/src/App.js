
import React from 'react';
import { Routes, Route } from 'react-router-dom'

import Home from "./components/home/index";
import Login from "./components/login/index";
import Signup from "./components/signup/index";
import Nav from "./components/nav/index"
import Profile from "./components/profile/index"
import LoginOrg from "./components/loginorg/index"
import Callback from './components/callback/index'
import Portal from './components/portal/index'

function App() {
  return (
    <div className="App">
      <Nav />
      <Routes>
        <Route path="/" element={<Home />} />
        <Route path="/account/login" element={<Login />} />
        <Route path="/account/signup" element={<Signup />} />
        <Route path="/account/profile" element={<Profile />} />
        <Route path="/account/loginorg" element={<LoginOrg />} />
        <Route path="/callback" element={<Callback />} />
        <Route path="/customer_portal" element={<Portal />} />
      </Routes>
    </div>
  );
}

export default App;
