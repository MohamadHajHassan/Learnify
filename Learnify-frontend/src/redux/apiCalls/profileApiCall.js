import { profileActions } from "./../slices/profileSlice";
import request from "../../utils/request";

export function getProfilePicture() {
    const user = localStorage.getItem("userInfo");
    const userId = user ? JSON.parse(user).id : null;
    return async dispatch => {
        try {
            const config = {
                headers: {
                    "Content-Type": "multipart/form-data",
                },
                responseType: "blob",
            };
            dispatch(profileActions.fetchProfileImageRequest());
            const response = await request.get(
                `/Users/${userId}/profilepicture`,
                config
            );
            const objectUrl = URL.createObjectURL(
                new Blob([response.data], {
                    type: response.headers["content-type"],
                })
            );
            dispatch(profileActions.fetchProfileImageSuccess(objectUrl));
        } catch (error) {
            dispatch(profileActions.fetchProfileImageFailure(error.response));
            console.error("Error fetching profile image:", error);
        }
    };
}
