import { createSlice } from "@reduxjs/toolkit";

const initialState = {
    profileImage: null,
    isLoading: false,
    error: null,
};

const profileSlice = createSlice({
    name: "profile",
    initialState,
    reducers: {
        fetchProfileImageRequest: state => {
            state.isLoading = true;
            state.error = null;
        },
        fetchProfileImageSuccess: (state, action) => {
            state.isLoading = false;
            state.profileImage = action.payload;
        },
        fetchProfileImageFailure: (state, action) => {
            state.isLoading = false;
            state.error = action.payload;
        },
    },
});

export const selectProfileImage = state => state.profile.profileImage;
export const selectIsProfileImageLoading = state => state.profile.isLoading;
export const selectProfileImageError = state => state.profile.error;

const profileReducer = profileSlice.reducer;
const profileActions = profileSlice.actions;

export { profileActions, profileReducer };
