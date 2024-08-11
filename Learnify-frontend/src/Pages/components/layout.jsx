import { Outlet } from "react-router-dom"
import Footer from "./footer"
import Navbar from "./Navbar"

const layout = () => {
  return (
    <>
      <Navbar />
      <Outlet />
      <Footer />
    </>
  )
}

export default layout
