import { validators } from "./validators";

describe("validateUsername", () => {
	it("returns false if length is less than 3", () => {
		expect(validators.validateUsername("")).toBe(false);
		expect(validators.validateUsername("s")).toBe(false);
		expect(validators.validateUsername("ds")).toBe(false);
		expect(validators.validateUsername("e")).toBe(false);
		expect(validators.validateUsername("54")).toBe(false);
	});

	it("returns false if the string contains white space", () => {
		expect(validators.validateUsername("   ")).toBe(false);
		expect(validators.validateUsername("sadfj lkasjdf")).toBe(false);
		expect(validators.validateUsername(" sds ")).toBe(false);
	});

	it("returns true if length is at least 3 and there is no white space", () => {
		expect(validators.validateUsername("asdfasd")).toBe(true);
		expect(validators.validateUsername("sadfjlkasjdf")).toBe(true);
		expect(validators.validateUsername("sds")).toBe(true);
		expect(validators.validateUsername("🥧🥊🧑")).toBe(true);
	});

	it("handles unicode correctly", () => {
		expect(validators.validateUsername("🥨")).toBe(false);
		expect(validators.validateUsername("🥌🦒")).toBe(false);
	});
});

describe("validatePassword", () => {
	it("returns false if length is less than 8", () => {
		expect(validators.validatePassword("4654654")).toBe(false);
		expect(validators.validatePassword("s")).toBe(false);
		expect(validators.validatePassword("dklj;ks")).toBe(false);
		expect(validators.validatePassword("565e")).toBe(false);
		expect(validators.validatePassword("5iouo4")).toBe(false);
	});

	it("returns false if the string contains white space", () => {
		expect(validators.validatePassword("   \n\t   ")).toBe(false);
		expect(validators.validatePassword("sadfj\tlkasjdf")).toBe(false);
		expect(validators.validatePassword(" s\nsdfg ds ")).toBe(false);
	});

	it("returns true if length is at least 8 and there is no white space", () => {
		expect(validators.validatePassword("asdfas4d")).toBe(true);
		expect(validators.validatePassword("sadfjlkasjdf")).toBe(true);
		expect(validators.validatePassword("s45625*()ds")).toBe(true);
		expect(validators.validatePassword("🥧🥊🧑🥅🥝🧓🧟🧦")).toBe(true);
	});

	it("handles unicode correctly", () => {
		expect(validators.validatePassword("🥨🥊🧑🥅")).toBe(false);
		expect(validators.validatePassword("🥌🦒🧟🧦🧟🧦🥊")).toBe(false);
	});
});

describe("validateToken", () => {
	it("returns false if value undefined", () => {
		expect(validators.validateToken(undefined)).toBe(false);
	});

	it("returns false if value invalid as base64(url)", () => {
		expect(validators.validateToken("asd")).toBe(false);
	});

	it("returns false if value not json", () => {
		expect(validators.validateToken("dnp4Y3Zj")).toBe(false);
	});

	it("returns false if there is no exp property", () => {
		expect(validators.validateToken("e30=")).toBe(false);
	});

	it("returns false if token expired", () => {
		const exp = Math.floor(Date.now() / 1000) - 10;
		const token = "blargyblargblarg." + btoa(JSON.stringify({ exp })).replace(/\+/g, "-").replace(/\//g, "_") + ".blargyasdlkujfd";
		expect(validators.validateToken(token)).toBe(false);
	});

	it("returns true if unexpired token", () => {
		const exp = Math.floor(Date.now() / 1000) + 10;
		const token = "blargyblargblarg." + btoa(JSON.stringify({ exp })).replace(/\+/g, "-").replace(/\//g, "_") + ".blargyasdlkujfd";
		expect(validators.validateToken(token)).toBe(true);
	});
});