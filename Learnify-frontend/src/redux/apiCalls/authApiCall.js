import { authActions } from "../slices/authSlice";
import request from "../../utils/request";
import { toast } from "react-toastify";
import swal from "sweetalert";

// Login User
export function loginUser(user) {
    return async dispatch => {
        try {
            const formData = new FormData();
            formData.append("Email", user.email);
            formData.append("Password", user.password);
            const config = {
                headers: {
                    "Content-Type": "multipart/form-data",
                },
            };
            const { data } = await request.post(
                "/Users/login",
                formData,
                config
            );

            dispatch(authActions.loginSuccess(data));

            // Get user enrollments
            localStorage.setItem("userInfo", JSON.stringify(data.user));
            localStorage.setItem("jwtToken", data.token);
            const enrollments = await request.get(
                `/Enrollments/userId/${data.user.id}`
            );
            localStorage.setItem(
                "enrollments",
                JSON.stringify(enrollments.data)
            );
        } catch (error) {
            dispatch(authActions.loginFailed(error.response.data));
            toast.error(error.response.data);
        }
    };
}

// Logout User
export function logoutUser() {
    return dispatch => {
        dispatch(authActions.logout());
        localStorage.removeItem("userInfo");
        localStorage.removeItem("jwtToken");
    };
}

// Register User
export function registerUser(user) {
    return async dispatch => {
        try {
            const formData = new FormData();
            formData.append("FirstName", user.fName);
            formData.append("LastName", user.lName);
            formData.append("Email", user.email);
            formData.append("Password", user.password);
            formData.append("ConfirmPassword", user.confirmPassword);
            const config = {
                headers: {
                    "Content-Type": "multipart/form-data",
                },
            };

            const { data } = await request.post(
                "/Users/register",
                formData,
                config
            );
            dispatch(authActions.register(data.message));
            swal({
                title: "Success",
                icon: "success",
            });
        } catch (error) {
            toast.error(error.response.data);
        }
    };
}
