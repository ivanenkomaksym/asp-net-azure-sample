import * as actionType from '../const/actionsTypes';

const authReducer = (state = { authData: null }, action) => {
  switch (action.type) {
    case actionType.AUTH:
        switch (action.identityProviderType) {
            case actionType.GOOGLE_IP:
                localStorage.setItem('user_info', JSON.stringify({ ...action?.data }));
                console.log("[authReducer] setting into local storage: ", action.data)
                return { ...state, authData: action.data };
            case actionType.MICROSOFT_IP:
                return {}
        }
    case actionType.LOGOUT:
      console.log("[authReducer] clearing local storage")
      localStorage.clear();

      return { ...state, authData: null };
    default:
      return state;
  }
};

export default authReducer;