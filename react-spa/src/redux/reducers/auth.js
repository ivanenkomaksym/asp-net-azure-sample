import * as actionType from '../const/actionsTypes';

const authReducer = (state = { authData: null }, action) => {
  switch (action.type) {
    case actionType.AUTH:
        saveUserToLocalStorage(action?.data);
        console.log("[authReducer] setting into local storage: ", action.data)
        return { ...state, authData: action.data };
    case actionType.LOGOUT:
      console.log("[authReducer] clearing local storage")
      localStorage.clear();
      return { ...state, authData: null };
    default:
      return state;
  }
};

export function saveUserToLocalStorage(user) {
    localStorage.setItem('user_info', JSON.stringify({ ...user }));
}

export function loadUserFromLocalStorage() {
    return JSON.parse(localStorage.getItem("user_info"));
}

export default authReducer;