import Courses from "./Courses"
import HelmetHandler from "./../../utils/HelmetHandler"
import "./home.css";


const Home = () => {
    return (
        <>
            <HelmetHandler title="HomePage" />
            <Courses />
        </>
    )
}

export default Home
