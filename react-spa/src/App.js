
import React from 'react';
import { Routes, Route } from 'react-router-dom'

import Home from "./components/home/index";
import Login from "./components/login/index";
import Signup from "./components/signup/index";
import Nav from "./components/nav/index"
import Profile from "./components/profile/index"
import LoginOrg from "./components/loginorg/index"

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
      </Routes>
    </div>
  );
}

export default App;
