import React from "react";
import {Link} from "react-router-dom"
import {connect} from "react-redux"
import {useEffect, useState} from "react";
import {useDispatch} from "react-redux"
import {LOGOUT} from "../../redux/const/actionsTypes"
import { useLocation } from 'react-router-dom'
import { useNavigate  } from 'react-router-dom';

function Nav(props) {
    const dispatch = useDispatch();
    const [userData, setUserData] = useState(null);
    const location = useLocation();
    const navigate = useNavigate (); // Get the history object

    useEffect(() => {
        const localUser = JSON.parse(localStorage.getItem("user_info"));
        if (localUser) {
            setUserData(localUser);
        } else {
            setUserData(null);
        }
    }, [location]);

    function handleLogOut(e) {
        e.preventDefault();
        dispatch({ type: LOGOUT });
        navigate('/'); // Navigate to the home page after logging out
    }

    return (
        <nav className="mainNav">
            <div className="navContainer">
                <Link className="linkBTN" to="/">Home</Link>
            </div>
            <div>
                {userData ? (
                    <div className="rightSideNav">
                        <i className="fa-solid fa-user"></i>
                        <div>
                            <span className="accountText">Account</span>
                            <div className="navContainer">
                                <Link className="linkBTN" to="/account/profile">Profile</Link>
                                <span className="orText">or</span>
                                <Link onClick={handleLogOut} className="linkBTN" to="/">Logout</Link>
                            </div>
                        </div>
                    </div>
                ) : (
                    <div className="rightSideNav">
                        <i className="fa-solid fa-user"></i>
                        <div>
                            <span className="accountText">Account</span>
                            <div className="navContainer">
                                <Link className="linkBTN" to="/account/login">Login</Link>
                                <span className="orText">or</span>
                                <Link className="linkBTN" to="/account/signup">Signup</Link>
                            </div>
                        </div>
                    </div>
                )}
            </div>
        </nav>
    );
}

const mapStateToProps = state => ({auth: state.auth});

export default connect(mapStateToProps)(Nav);