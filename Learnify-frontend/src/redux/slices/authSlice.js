import { createSlice } from "@reduxjs/toolkit";

const authSlice = createSlice({
    name: "auth",
    initialState: {
        user: localStorage.getItem("userInfo")
            ? JSON.parse(localStorage.getItem("userInfo"))
            : null,
        registerMessage: null,
        loginError: null,
    },
    reducers: {
        loginSuccess(state, action) {
            state.user = action.payload.user;
            state.token = action.payload.token;
            state.loginError = null;
        },
        loginFailed(state, action) {
            state.user = null;
            state.loginError = action.payload;
        },
        logout(state) {
            state.user = null;
            state.token = null;
        },
        register(state, action) {
            state.registerMessage = action.payload;
        },
    },
});

const authReducer = authSlice.reducer;
const authActions = authSlice.actions;

export { authReducer, authActions };
